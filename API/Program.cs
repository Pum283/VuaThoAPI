using API.Helpers;
using Infrastructure.Persistence;
using API.Middlewares;
using Application;
using Infrastructure;
using Application.DTOs.Auth;
using Application.Interfaces.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Load .env for local development (không dùng cho production)
if (builder.Environment.IsDevelopment())
{
    var envPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
    if (File.Exists(envPath))
    {
        DotNetEnv.Env.Load(envPath);
        builder.Configuration.AddEnvironmentVariables();
    }
}

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vua Thợ API",
        Version = "v1",
        Description = "Tài liệu API Backend cho hệ thống Vua Thợ."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT theo định dạng: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Application Layer
builder.Services.AddApplication();

// Add Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Authentication (JWT)
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];
var signingKey = jwtSection["SigningKey"];

if (string.IsNullOrWhiteSpace(signingKey))
{
    throw new InvalidOperationException(
        "Thiếu cấu hình JWT signing key. Vui lòng thiết lập biến môi trường 'Jwt__SigningKey'.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 52_428_800; // ~50 MB tổng form đăng ký thợ + file
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("DefaultCors");

// Exception handling middleware
app.UseExceptionMiddleware();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Minimal APIs - Auth (v1)
var authGroup = app.MapGroup("/api/v1/auth")
    .WithTags("Xác thực");

authGroup.MapPost("/send-otp",
        async (SendOtpRequest request, IValidator<SendOtpRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var result = await authService.SendOtpAsync(request, ct);
            return result.IsSuccess
                ? Results.Ok(new { message = result.Message ?? "Đã gửi OTP." })
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Gửi OTP thất bại." });
        })
    .WithSummary("Gửi OTP")
    .WithDescription("Gửi OTP đến số điện thoại để xác thực trước khi đăng ký.")
    .AllowAnonymous();

authGroup.MapPost("/send-otp-email",
        async (SendEmailOtpRequest request, IValidator<SendEmailOtpRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var result = await authService.SendEmailOtpAsync(request, ct);
            return result.IsSuccess
                ? Results.Ok(new { message = result.Message ?? "Đã gửi OTP qua email." })
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Gửi OTP thất bại." });
        })
    .WithSummary("Gửi OTP qua email")
    .WithDescription("Gửi OTP đến email để xác thực trước khi đăng ký.")
    .AllowAnonymous();

authGroup.MapPost("/verify-otp",
        async (VerifyOtpRequest request, IValidator<VerifyOtpRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var result = await authService.VerifyOtpAsync(request, ct);
            return result.IsSuccess && result.Data != null
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Xác thực OTP thất bại." });
        })
    .WithSummary("Xác thực OTP")
    .WithDescription("Xác thực OTP và trả về token xác thực để dùng cho bước đăng ký.")
    .AllowAnonymous();

authGroup.MapPost("/verify-otp-email",
        async (VerifyEmailOtpRequest request, IValidator<VerifyEmailOtpRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var result = await authService.VerifyEmailOtpAsync(request, ct);
            return result.IsSuccess && result.Data != null
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Xác thực OTP thất bại." });
        })
    .WithSummary("Xác thực OTP qua email")
    .WithDescription("Xác thực OTP gửi qua email và trả về token xác thực để dùng cho bước đăng ký.")
    .AllowAnonymous();

authGroup.MapPost("/register/customer",
        async (RegisterCustomerRequest request, IValidator<RegisterCustomerRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var result = await authService.RegisterCustomerAsync(request, ct);
            return result.IsSuccess && result.Data != null
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Đăng ký thất bại." });
        })
    .WithSummary("Đăng ký khách hàng")
    .WithDescription("Đăng ký tài khoản khách hàng (Active ngay sau đăng ký).")
    .AllowAnonymous();

authGroup.MapPost("/register/worker-external",
        async (HttpRequest httpRequest, IValidator<RegisterWorkerExternalRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            if (!httpRequest.HasFormContentType)
            {
                return Results.BadRequest(new[] { "Yêu cầu gửi multipart/form-data (kèm file đăng ký)." });
            }

            var form = await httpRequest.ReadFormAsync(ct);
            var request = WorkerRegistrationMultipartReader.ReadExternalFields(form);
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var files = WorkerRegistrationMultipartReader.ReadExternalFiles(form);
            var result = await authService.RegisterWorkerExternalAsync(request, files, ct);
            return result.IsSuccess && result.Data != null
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Đăng ký thất bại." });
        })
    .WithSummary("Đăng ký thợ ngoài")
    .WithDescription("Đăng ký thợ ngoài (multipart): otpVerificationToken, phoneNumber, email, password, fullName; file: diploma (tuỳ chọn), identityCard (bắt buộc), otherDocuments (lặp lại, tuỳ chọn). PDF/JPG/PNG/WEBP, tối đa 10 MB/file.")
    .DisableAntiforgery()
    .AllowAnonymous();

authGroup.MapPost("/register/worker-direct",
        async (HttpRequest httpRequest, IValidator<RegisterWorkerDirectRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            if (!httpRequest.HasFormContentType)
            {
                return Results.BadRequest(new[] { "Yêu cầu gửi multipart/form-data (kèm file đăng ký)." });
            }

            var form = await httpRequest.ReadFormAsync(ct);
            var request = WorkerRegistrationMultipartReader.ReadDirectFields(form);
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var files = WorkerRegistrationMultipartReader.ReadDirectFiles(form);
            var result = await authService.RegisterWorkerDirectAsync(request, files, ct);
            return result.IsSuccess && result.Data != null
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Đăng ký thất bại." });
        })
    .WithSummary("Đăng ký thợ trực thuộc")
    .WithDescription("Đăng ký thợ công ty (multipart): các field text như thợ ngoài; file cv (bắt buộc). PDF/JPG/PNG/WEBP, tối đa 10 MB.")
    .DisableAntiforgery()
    .AllowAnonymous();

authGroup.MapPost("/login",
        async (LoginRequest request, IValidator<LoginRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var result = await authService.LoginAsync(request, ct);
            return result.IsSuccess && result.Data != null
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Đăng nhập thất bại." });
        })
    .WithSummary("Đăng nhập")
    .WithDescription("Đăng nhập bằng số điện thoại hoặc email + mật khẩu. Trả về access token + refresh token + user info.")
    .AllowAnonymous();

authGroup.MapPost("/refresh-token",
        async (RefreshTokenRequest request, IValidator<RefreshTokenRequest> validator, IAuthService authService, CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var result = await authService.RefreshTokenAsync(request, ct);
            return result.IsSuccess && result.Data != null
                ? Results.Ok(result.Data)
                : Results.BadRequest(result.Errors.Any() ? result.Errors : new[] { result.Message ?? "Làm mới token thất bại." });
        })
    .WithSummary("Làm mới access token")
    .WithDescription("Cấp lại access token bằng refresh token, đồng thời rotate refresh token.")
    .AllowAnonymous();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (context.Database.IsInMemory())
        {
            context.Database.EnsureCreated();
        }
        else
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi khi khởi tạo cơ sở dữ liệu.");
    }
}

app.Run();
