using Application.Common;
using Application.Constants;
using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan OtpVerificationTokenTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan RefreshTokenTtl = TimeSpan.FromDays(30);

        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailService _emailService;
        private readonly IWorkerRegistrationFileStorage _workerRegistrationFileStorage;

        public AuthService(
            AppDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IEmailService emailService,
            IJwtTokenService jwtTokenService,
            IWorkerRegistrationFileStorage workerRegistrationFileStorage)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _jwtTokenService = jwtTokenService;
            _workerRegistrationFileStorage = workerRegistrationFileStorage;
        }

        public async Task<OperationResult> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
        {
            var phoneNumber = request.PhoneNumber.Trim();

            // OTP demo: generate 6 digits. Thực tế: tích hợp SMS provider.
            var otp = Random.Shared.Next(100000, 999999).ToString();
            var (hash, salt) = CryptoHelper.HashWithRandomSalt(otp);

            var entity = new OtpCode
            {
                PhoneNumber = phoneNumber,
                CodeHash = hash,
                Salt = salt,
                ExpiresAtUtc = DateTime.UtcNow.Add(OtpTtl),
                IsVerified = false,
                FailedAttemptCount = 0
            };

            _context.OtpCodes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Lưu ý: Không trả OTP ra response. Trong môi trường dev, có thể log.
            return OperationResult.Success("Đã gửi OTP. Vui lòng kiểm tra tin nhắn.");
        }

        public async Task<OperationResult> SendEmailOtpAsync(SendEmailOtpRequest request, CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim();

            var otp = Random.Shared.Next(100000, 999999).ToString();
            var (hash, salt) = CryptoHelper.HashWithRandomSalt(otp);

            var entity = new OtpCode
            {
                PhoneNumber = string.Empty,
                Email = email,
                CodeHash = hash,
                Salt = salt,
                ExpiresAtUtc = DateTime.UtcNow.Add(OtpTtl),
                IsVerified = false,
                FailedAttemptCount = 0
            };

            _context.OtpCodes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            await _emailService.SendOtpAsync(email, otp, cancellationToken);
            return OperationResult.Success("Đã gửi OTP qua email. Vui lòng kiểm tra hộp thư.");
        }

        public async Task<OperationResult<VerifyOtpResponse>> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
        {
            var phoneNumber = request.PhoneNumber.Trim();
            var otp = request.OtpCode.Trim();

            var otpEntity = await _context.OtpCodes
                .Where(x => x.PhoneNumber == phoneNumber && !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (otpEntity == null)
            {
                return OperationResult<VerifyOtpResponse>.Failure("OTP không tồn tại hoặc đã hết hạn.");
            }

            if (otpEntity.ExpiresAtUtc < DateTime.UtcNow)
            {
                return OperationResult<VerifyOtpResponse>.Failure("OTP đã hết hạn. Vui lòng yêu cầu OTP mới.");
            }

            if (otpEntity.IsVerified)
            {
                if (!string.IsNullOrWhiteSpace(otpEntity.VerificationToken) && otpEntity.VerificationTokenExpiresAtUtc > DateTime.UtcNow)
                {
                    return OperationResult<VerifyOtpResponse>.Success(new VerifyOtpResponse
                    {
                        OtpVerificationToken = otpEntity.VerificationToken,
                        ExpiresAtUtc = otpEntity.VerificationTokenExpiresAtUtc!.Value
                    }, "OTP đã được xác thực.");
                }
            }

            var isValid = CryptoHelper.VerifySha256(otp, otpEntity.Salt, otpEntity.CodeHash);
            if (!isValid)
            {
                otpEntity.FailedAttemptCount += 1;
                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult<VerifyOtpResponse>.Failure("OTP không đúng.");
            }

            otpEntity.IsVerified = true;
            otpEntity.VerifiedAtUtc = DateTime.UtcNow;
            otpEntity.VerificationToken = CryptoHelper.GenerateSecureToken(24);
            otpEntity.VerificationTokenExpiresAtUtc = DateTime.UtcNow.Add(OtpVerificationTokenTtl);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<VerifyOtpResponse>.Success(new VerifyOtpResponse
            {
                OtpVerificationToken = otpEntity.VerificationToken,
                ExpiresAtUtc = otpEntity.VerificationTokenExpiresAtUtc.Value
            }, "Xác thực OTP thành công.");
        }

        public async Task<OperationResult<VerifyOtpResponse>> VerifyEmailOtpAsync(VerifyEmailOtpRequest request, CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim();
            var otp = request.OtpCode.Trim();

            var otpEntity = await _context.OtpCodes
                .Where(x => x.Email == email && !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (otpEntity == null)
            {
                return OperationResult<VerifyOtpResponse>.Failure("OTP không tồn tại hoặc đã hết hạn.");
            }

            if (otpEntity.ExpiresAtUtc < DateTime.UtcNow)
            {
                return OperationResult<VerifyOtpResponse>.Failure("OTP đã hết hạn. Vui lòng yêu cầu OTP mới.");
            }

            if (otpEntity.IsVerified)
            {
                if (!string.IsNullOrWhiteSpace(otpEntity.VerificationToken) &&
                    otpEntity.VerificationTokenExpiresAtUtc is { } tokenExp &&
                    tokenExp > DateTime.UtcNow)
                {
                    return OperationResult<VerifyOtpResponse>.Success(new VerifyOtpResponse
                    {
                        OtpVerificationToken = otpEntity.VerificationToken,
                        ExpiresAtUtc = tokenExp
                    }, "OTP đã được xác thực.");
                }

                return OperationResult<VerifyOtpResponse>.Failure("OTP đã được sử dụng. Vui lòng yêu cầu mã mới.");
            }

            var isValid = CryptoHelper.VerifySha256(otp, otpEntity.Salt, otpEntity.CodeHash);
            if (!isValid)
            {
                otpEntity.FailedAttemptCount += 1;
                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult<VerifyOtpResponse>.Failure("OTP không đúng.");
            }

            otpEntity.IsVerified = true;
            otpEntity.VerifiedAtUtc = DateTime.UtcNow;
            otpEntity.VerificationToken = CryptoHelper.GenerateSecureToken(24);
            otpEntity.VerificationTokenExpiresAtUtc = DateTime.UtcNow.Add(OtpVerificationTokenTtl);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<VerifyOtpResponse>.Success(new VerifyOtpResponse
            {
                OtpVerificationToken = otpEntity.VerificationToken,
                ExpiresAtUtc = otpEntity.VerificationTokenExpiresAtUtc.Value
            }, "Xác thực OTP thành công.");
        }

        public Task<OperationResult<AuthResponse>> RegisterCustomerAsync(RegisterCustomerRequest request, CancellationToken cancellationToken = default)
        {
            return RegisterInternalAsync(
                roleName: RoleNames.Customer,
                workerType: null,
                workerStatus: null,
                phoneNumber: request.PhoneNumber,
                email: request.Email,
                fullName: request.FullName,
                password: request.Password,
                otpVerificationToken: request.OtpVerificationToken,
                latitude: null,
                longitude: null,
                registrationFiles: null,
                cancellationToken: cancellationToken);
        }

        public async Task<OperationResult<AuthResponse>> RegisterWorkerExternalAsync(
            RegisterWorkerExternalRequest request,
            IReadOnlyList<WorkerRegistrationFileUpload> registrationFiles,
            CancellationToken cancellationToken = default)
        {
            var files = registrationFiles ?? Array.Empty<WorkerRegistrationFileUpload>();
            var fileList = files.ToList();
            try
            {
                var docErrors = WorkerRegistrationFileRules.ValidateRequiredDocumentsForExternal(fileList);
                if (docErrors.Count > 0)
                {
                    return OperationResult<AuthResponse>.Failure(docErrors.ToList());
                }

                return await RegisterInternalAsync(
                    roleName: RoleNames.WorkerExternal,
                    workerType: WorkerType.External,
                    workerStatus: WorkerStatus.Pending,
                    phoneNumber: request.PhoneNumber,
                    email: request.Email,
                    fullName: request.FullName,
                    password: request.Password,
                    otpVerificationToken: request.OtpVerificationToken,
                    latitude: null,
                    longitude: null,
                    registrationFiles: fileList,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                foreach (var f in fileList)
                {
                    await f.Content.DisposeAsync();
                }
            }
        }

        public async Task<OperationResult<AuthResponse>> RegisterWorkerDirectAsync(
            RegisterWorkerDirectRequest request,
            IReadOnlyList<WorkerRegistrationFileUpload> registrationFiles,
            CancellationToken cancellationToken = default)
        {
            var files = registrationFiles ?? Array.Empty<WorkerRegistrationFileUpload>();
            var fileList = files.ToList();
            try
            {
                var docErrors = WorkerRegistrationFileRules.ValidateRequiredDocumentsForDirect(fileList);
                if (docErrors.Count > 0)
                {
                    return OperationResult<AuthResponse>.Failure(docErrors.ToList());
                }

                return await RegisterInternalAsync(
                    roleName: RoleNames.WorkerDirect,
                    workerType: WorkerType.Direct,
                    workerStatus: WorkerStatus.Inactive,
                    phoneNumber: request.PhoneNumber,
                    email: request.Email,
                    fullName: request.FullName,
                    password: request.Password,
                    otpVerificationToken: request.OtpVerificationToken,
                    latitude: null,
                    longitude: null,
                    registrationFiles: fileList,
                    cancellationToken: cancellationToken);
            }
            finally
            {
                foreach (var f in fileList)
                {
                    await f.Content.DisposeAsync();
                }
            }
        }

        public async Task<OperationResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var identifier = request.Identifier.Trim();
            var password = request.Password;

            var user = await _userManager.Users
                .Include(x => x.WorkerProfile)
                .FirstOrDefaultAsync(x =>
                    x.PhoneNumber == identifier ||
                    (x.Email != null && x.Email == identifier),
                    cancellationToken);

            if (user == null)
            {
                return OperationResult<AuthResponse>.Failure("Tài khoản hoặc mật khẩu không đúng.");
            }

            if (!user.IsActive || user.IsDeleted)
            {
                return OperationResult<AuthResponse>.Failure("Tài khoản đang bị khoá hoặc không khả dụng.");
            }

            var ok = await _userManager.CheckPasswordAsync(user, password);
            if (!ok)
            {
                return OperationResult<AuthResponse>.Failure("Tài khoản hoặc mật khẩu không đúng.");
            }

            var (refreshTokenPlain, _) = await IssueRefreshTokenAsync(user.Id, createdIp: null, cancellationToken);

            var workerType = user.WorkerProfile?.WorkerType;
            var workerStatus = user.WorkerProfile?.WorkerStatus;
            var role = await GetPrimaryRoleAsync(user, cancellationToken);
            var accessToken = _jwtTokenService.CreateAccessToken(user, role, workerType, workerStatus);

            return OperationResult<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenPlain,
                UserInfo = await MapUserInfoAsync(user, cancellationToken)
            }, "Đăng nhập thành công.");
        }

        public async Task<OperationResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var refreshTokenPlain = request.RefreshToken.Trim();
            if (string.IsNullOrWhiteSpace(refreshTokenPlain))
            {
                return OperationResult<AuthResponse>.Failure("Refresh token không hợp lệ.");
            }

            // Find token by scanning recent tokens (simple approach). Tối ưu sau: thêm lookup key.
            var candidates = await _context.RefreshTokens
                .Where(x => !x.IsDeleted && x.RevokedAtUtc == null && x.ExpiresAtUtc > DateTime.UtcNow)
                .OrderByDescending(x => x.Id)
                .Take(200)
                .ToListAsync(cancellationToken);

            RefreshToken? matched = null;
            foreach (var token in candidates)
            {
                if (CryptoHelper.VerifySha256(refreshTokenPlain, token.Salt, token.TokenHash))
                {
                    matched = token;
                    break;
                }
            }

            if (matched == null)
            {
                return OperationResult<AuthResponse>.Failure("Refresh token không hợp lệ hoặc đã hết hạn.");
            }

            var user = await _userManager.Users
                .Include(x => x.WorkerProfile)
                .FirstOrDefaultAsync(x => x.Id == matched.UserId, cancellationToken);

            if (user == null || user.IsDeleted || !user.IsActive)
            {
                return OperationResult<AuthResponse>.Failure("Tài khoản không khả dụng.");
            }

            matched.RevokedAtUtc = DateTime.UtcNow;
            matched.RevokeReason = "Rotate refresh token";

            var (newRefreshTokenPlain, _) = await IssueRefreshTokenAsync(user.Id, createdIp: null, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var workerType = user.WorkerProfile?.WorkerType;
            var workerStatus = user.WorkerProfile?.WorkerStatus;
            var role = await GetPrimaryRoleAsync(user, cancellationToken);
            var accessToken = _jwtTokenService.CreateAccessToken(user, role, workerType, workerStatus);

            return OperationResult<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenPlain,
                UserInfo = await MapUserInfoAsync(user, cancellationToken)
            }, "Làm mới token thành công.");
        }

        private async Task<OperationResult<AuthResponse>> RegisterInternalAsync(
            string roleName,
            WorkerType? workerType,
            WorkerStatus? workerStatus,
            string phoneNumber,
            string? email,
            string fullName,
            string password,
            string otpVerificationToken,
            decimal? latitude,
            decimal? longitude,
            IReadOnlyList<WorkerRegistrationFileUpload>? registrationFiles,
            CancellationToken cancellationToken)
        {
            var phone = phoneNumber.Trim();
            var emailNormalized = string.IsNullOrWhiteSpace(email) ? null : email.Trim();

            var otpEntity = await _context.OtpCodes
                .Where(x =>
                    x.IsVerified &&
                    !x.IsDeleted &&
                    x.VerificationToken != null &&
                    x.VerificationToken == otpVerificationToken &&
                    x.VerificationTokenExpiresAtUtc != null &&
                    x.VerificationTokenExpiresAtUtc > DateTime.UtcNow)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (otpEntity == null)
            {
                return OperationResult<AuthResponse>.Failure("Bạn cần xác thực OTP trước khi đăng ký, hoặc token xác thực đã hết hạn.");
            }

            if (!string.IsNullOrWhiteSpace(otpEntity.Email))
            {
                if (string.IsNullOrWhiteSpace(emailNormalized))
                {
                    return OperationResult<AuthResponse>.Failure("Email là bắt buộc sau khi xác thực OTP qua email.");
                }

                if (!string.Equals(otpEntity.Email.Trim(), emailNormalized, StringComparison.OrdinalIgnoreCase))
                {
                    return OperationResult<AuthResponse>.Failure("Email đăng ký phải trùng email đã xác thực OTP.");
                }
            }
            else
            {
                if (!string.Equals((otpEntity.PhoneNumber ?? string.Empty).Trim(), phone, StringComparison.Ordinal))
                {
                    return OperationResult<AuthResponse>.Failure("Số điện thoại đăng ký phải trùng số đã xác thực OTP.");
                }
            }

            var existedPhone = await _userManager.Users.AnyAsync(x => x.PhoneNumber == phone, cancellationToken);
            if (existedPhone)
            {
                return OperationResult<AuthResponse>.Failure("Số điện thoại đã được sử dụng.");
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var existedEmail = await _userManager.Users.AnyAsync(x => x.Email == email, cancellationToken);
                if (existedEmail)
                {
                    return OperationResult<AuthResponse>.Failure("Email đã được sử dụng.");
                }
            }

            var user = new User
            {
                PhoneNumber = phone,
                UserName = phone,
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                FullName = fullName.Trim(),
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(x => x.Description).ToList();
                return OperationResult<AuthResponse>.Failure(errors);
            }

            await EnsureRoleExistsAsync(roleName, cancellationToken);
            var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addRoleResult.Succeeded)
            {
                var errors = addRoleResult.Errors.Select(x => x.Description).ToList();
                return OperationResult<AuthResponse>.Failure(errors);
            }

            WorkerProfile? createdWorkerProfile = null;
            if (workerType.HasValue && workerStatus.HasValue)
            {
                createdWorkerProfile = new WorkerProfile
                {
                    UserId = user.Id,
                    WorkerType = workerType.Value,
                    WorkerStatus = workerStatus.Value,
                    Latitude = latitude ?? 0,
                    Longitude = longitude ?? 0,
                    RatingAvg = 0,
                    RatingCount = 0,
                    BasePrice = 0
                };
                _context.WorkerProfiles.Add(createdWorkerProfile);
            }

            // invalidate token after use
            otpEntity.VerificationToken = null;
            otpEntity.VerificationTokenExpiresAtUtc = null;

            var (refreshTokenPlain, _) = await IssueRefreshTokenAsync(user.Id, createdIp: null, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            if (createdWorkerProfile != null &&
                registrationFiles is { Count: > 0 })
            {
                foreach (var file in registrationFiles)
                {
                    var relativePath = await _workerRegistrationFileStorage.SaveAsync(
                        createdWorkerProfile.Id,
                        file.OriginalFileName,
                        file.Content,
                        cancellationToken);

                    _context.WorkerRegistrationDocuments.Add(new WorkerRegistrationDocument
                    {
                        WorkerProfileId = createdWorkerProfile.Id,
                        DocumentType = file.DocumentType,
                        OriginalFileName = file.OriginalFileName,
                        StoredRelativePath = relativePath,
                        ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                        FileSizeBytes = file.Length
                    });
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            var accessToken = _jwtTokenService.CreateAccessToken(user, roleName, workerType, workerStatus);

            return OperationResult<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenPlain,
                UserInfo = new Application.DTOs.Auth.UserInfoResponse
                {
                    UserId = user.Id,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = roleName,
                    WorkerType = workerType,
                    WorkerStatus = workerStatus
                }
            }, "Đăng ký thành công.");
        }

        private async Task<Application.DTOs.Auth.UserInfoResponse> MapUserInfoAsync(User user, CancellationToken cancellationToken)
        {
            var role = await GetPrimaryRoleAsync(user, cancellationToken);
            return new Application.DTOs.Auth.UserInfoResponse
            {
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = user.Email,
                FullName = user.FullName,
                Role = role,
                WorkerType = user.WorkerProfile?.WorkerType,
                WorkerStatus = user.WorkerProfile?.WorkerStatus
            };
        }

        private async Task<string> GetPrimaryRoleAsync(User user, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count == 0)
            {
                return string.Empty;
            }

            // Ưu tiên Admin nếu có, còn lại lấy role đầu tiên
            if (roles.Contains(RoleNames.Admin))
            {
                return RoleNames.Admin;
            }

            return roles[0];
        }

        private async Task EnsureRoleExistsAsync(string roleName, CancellationToken cancellationToken)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return;
            }

            var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Không thể khởi tạo role hệ thống. Vui lòng thử lại.");
            }
        }

        private async Task<(string refreshTokenPlain, RefreshToken refreshTokenEntity)> IssueRefreshTokenAsync(
            int userId,
            string? createdIp,
            CancellationToken cancellationToken)
        {
            var tokenPlain = CryptoHelper.GenerateSecureToken(48);
            var (hash, salt) = CryptoHelper.HashWithRandomSalt(tokenPlain);

            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = hash,
                Salt = salt,
                ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenTtl),
                CreatedIp = createdIp
            };

            _context.RefreshTokens.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return (tokenPlain, entity);
        }
    }
}

