using Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace API.Middlewares
{
    /// <summary>
    /// Middleware xử lý exception toàn cục, trả về lỗi theo JSON.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// Khởi tạo middleware.
        /// </summary>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Thực thi middleware.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            // Check specific exceptions first (NotFoundException inherits from BusinessException)
            if (exception is NotFoundException notFoundException)
            {
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new { error = notFoundException.Message });
            }
            else if (exception is BusinessException businessException)
            {
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { error = businessException.Message });
            }
            else
            {
                result = JsonSerializer.Serialize(new { error = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu." });
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Extensions để đăng ký middleware xử lý exception.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Đăng ký middleware xử lý exception toàn cục.
        /// </summary>
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}

