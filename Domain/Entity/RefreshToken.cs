using Domain.Common;

namespace Domain.Entity
{
    /// <summary>
    /// Refresh token để cấp lại access token.
    /// </summary>
    public class RefreshToken : BaseAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Định danh người dùng sở hữu refresh token.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Hash của refresh token (không lưu token plain text).
        /// </summary>
        public string TokenHash { get; set; } = string.Empty;

        /// <summary>
        /// Salt dùng để hash refresh token.
        /// </summary>
        public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// Thời điểm hết hạn (UTC).
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>
        /// Thời điểm thu hồi (UTC) (nếu có).
        /// </summary>
        public DateTime? RevokedAtUtc { get; set; }

        /// <summary>
        /// Lý do thu hồi (tiếng Việt).
        /// </summary>
        public string? RevokeReason { get; set; }

        /// <summary>
        /// IP tạo token (nếu có).
        /// </summary>
        public string? CreatedIp { get; set; }

        /// <summary>
        /// IP thu hồi token (nếu có).
        /// </summary>
        public string? RevokedIp { get; set; }

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }
    }
}

