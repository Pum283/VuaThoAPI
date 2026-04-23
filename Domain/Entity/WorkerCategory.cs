using Domain.Common;

namespace Domain.Entity
{
    /// <summary>
    /// Bảng liên kết nhiều-nhiều giữa thợ và danh mục dịch vụ.
    /// </summary>
    public class WorkerCategory : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// Khoá ngoại đến hồ sơ thợ.
        /// </summary>
        public Guid WorkerProfileId { get; set; }

        /// <summary>
        /// Hồ sơ thợ.
        /// </summary>
        public WorkerProfile? WorkerProfile { get; set; }

        /// <summary>
        /// Khoá ngoại đến danh mục.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Danh mục dịch vụ.
        /// </summary>
        public Category? Category { get; set; }

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }
    }
}

