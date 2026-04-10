namespace Application.DTOs.Auth
{
    /// <summary>
    /// Yêu cầu gửi OTP đến số điện thoại.
    /// </summary>
    public class SendOtpRequest
    {
        /// <summary>
        /// Số điện thoại nhận OTP.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
    }
}

