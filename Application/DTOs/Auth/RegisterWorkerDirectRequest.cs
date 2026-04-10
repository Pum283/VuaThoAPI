namespace Application.DTOs.Auth
{
    /// <summary>
    /// Đăng ký tài khoản thợ trực thuộc (chờ phỏng vấn offline).
    /// </summary>
    public class RegisterWorkerDirectRequest
    {
        /// <summary>
        /// Token xác thực OTP (nhận được sau bước verify OTP).
        /// </summary>
        public string OtpVerificationToken { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại (đã xác thực OTP).
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Email (không bắt buộc).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Mật khẩu.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Họ và tên hiển thị.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Vĩ độ (Latitude).
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Kinh độ (Longitude).
        /// </summary>
        public decimal Longitude { get; set; }
    }
}

