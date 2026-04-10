using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    /// <summary>
    /// Hồ sơ thợ.
    /// </summary>
    public class WorkerProfile : BaseAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Liên kết đến người dùng.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Người dùng sở hữu hồ sơ thợ.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Loại thợ (trực thuộc/ngoài).
        /// </summary>
        public WorkerType WorkerType { get; set; }

        /// <summary>
        /// Trạng thái hồ sơ thợ.
        /// </summary>
        public WorkerStatus WorkerStatus { get; set; }

        /// <summary>
        /// Vĩ độ (Latitude).
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Kinh độ (Longitude).
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Điểm đánh giá trung bình.
        /// </summary>
        public decimal RatingAvg { get; set; }

        /// <summary>
        /// Số lượng đánh giá.
        /// </summary>
        public int RatingCount { get; set; }

        /// <summary>
        /// Giá cơ bản (nếu áp dụng).
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Danh mục dịch vụ thợ có thể nhận.
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

