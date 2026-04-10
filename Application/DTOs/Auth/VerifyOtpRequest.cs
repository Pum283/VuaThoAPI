namespace Application.DTOs.Auth
{
    /// <summary>
    /// Yêu cầu xác thực OTP.
    /// </summary>
    public class VerifyOtpRequest
    {
        /// <summary>
        /// Số điện thoại đã nhận OTP.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Mã OTP.
        /// </summary>
        public string OtpCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả xác thực OTP (dùng để tiếp tục đăng ký).
    /// </summary>
    public class VerifyOtpResponse
    {
        /// <summary>
        /// Token xác thực OTP, dùng cho bước đăng ký.
        /// </summary>
        public string OtpVerificationToken { get; set; } = string.Empty;

        /// <summary>
        /// Hết hạn lúc (UTC).
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }
    }
}

