using Domain.Common;

namespace Domain.Entity
{
    /// <summary>
    /// Mã OTP phục vụ xác thực số điện thoại khi đăng ký/đăng nhập.
    /// </summary>
    public class OtpCode : BaseAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Số điện thoại nhận OTP.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Email nhận OTP (nếu gửi qua email).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Hash của OTP (không lưu OTP plain text).
        /// </summary>
        public string CodeHash { get; set; } = string.Empty;

        /// <summary>
        /// Salt dùng để hash OTP.
        /// </summary>
        public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// Thời điểm hết hạn (UTC).
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>
        /// Đã xác thực thành công hay chưa.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Thời điểm xác thực (UTC) (nếu có).
        /// </summary>
        public DateTime? VerifiedAtUtc { get; set; }

        /// <summary>
        /// Số lần nhập sai OTP.
        /// </summary>
        public int FailedAttemptCount { get; set; }

        /// <summary>
        /// Token xác thực dùng cho bước đăng ký (được trả về sau khi verify OTP).
        /// </summary>
        public string? VerificationToken { get; set; }

        /// <summary>
        /// Thời điểm token xác thực hết hạn (UTC) (nếu có).
        /// </summary>
        public DateTime? VerificationTokenExpiresAtUtc { get; set; }

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }
    }
}

