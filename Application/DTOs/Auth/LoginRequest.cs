namespace Application.DTOs.Auth
{
    /// <summary>
    /// Yêu cầu đăng nhập.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Số điện thoại hoặc email.
        /// </summary>
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}

