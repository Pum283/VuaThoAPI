namespace Application.DTOs.Auth
{
    /// <summary>
    /// Yêu cầu xác thực OTP qua email.
    /// </summary>
    public class VerifyEmailOtpRequest
    {
        /// <summary>
        /// Email đã nhận OTP.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mã OTP.
        /// </summary>
        public string OtpCode { get; set; } = string.Empty;
    }
}

