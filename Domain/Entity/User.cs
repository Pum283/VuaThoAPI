using Domain.Common;

namespace Domain.Entity
{
    /// <summary>
    /// Người dùng trong hệ thống (lớp nền tảng).
    /// </summary>
    public class User : BaseAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Số điện thoại đăng nhập.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Họ và tên hiển thị.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email (nếu có).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Trạng thái hoạt động.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }

        /// <summary>
        /// Hồ sơ thợ (nếu người dùng là thợ).
        /// </summary>
        public WorkerProfile? WorkerProfile { get; set; }
    }
}

