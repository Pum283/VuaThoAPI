using Domain.Common;

namespace Domain.Entity
{
    /// <summary>
    /// Ngành nghề/danh mục dịch vụ.
    /// </summary>
    public class Category : BaseAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Tên danh mục.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả (nếu có).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Quan hệ nhiều-nhiều với hồ sơ thợ.
        /// </summary>
        public ICollection<WorkerCategory> WorkerCategories { get; set; } = new List<WorkerCategory>();

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }
    }
}

