using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entity
{
    /// <summary>
    /// Người dùng trong hệ thống (lớp nền tảng).
    /// </summary>
    public class User : IdentityUser<Guid>, IAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Họ và tên hiển thị.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái hoạt động.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Thời điểm tạo (UTC).
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Người tạo.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Thời điểm cập nhật (UTC).
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Người cập nhật.
        /// </summary>
        public string? UpdatedBy { get; set; }

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

