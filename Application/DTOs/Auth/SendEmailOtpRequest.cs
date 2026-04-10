namespace Application.DTOs.Auth
{
    /// <summary>
    /// Yêu cầu gửi OTP qua email.
    /// </summary>
    public class SendEmailOtpRequest
    {
        /// <summary>
        /// Email nhận OTP.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}

