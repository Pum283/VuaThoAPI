namespace Application.DTOs.Auth
{
    /// <summary>
    /// Đăng ký tài khoản thợ (cần admin duyệt hồ sơ).
    /// </summary>
    public class RegisterWorkerExternalRequest
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
        /// Email (bắt buộc; phải trùng email đã xác thực OTP qua email).
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
        /// Danh sách chuyên môn (danh mục) mà thợ đăng ký (chọn 1 hoặc nhiều).
        /// </summary>
        public IReadOnlyList<Guid> CategoryIds { get; set; } = new List<Guid>();
    }
}

