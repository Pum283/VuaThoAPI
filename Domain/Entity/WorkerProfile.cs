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
        public Guid UserId { get; set; }

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
        /// Admin xử lý hồ sơ này (nếu đã xử lý).
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Thời điểm admin xử lý hồ sơ (UTC).
        /// </summary>
        public DateTime? ReviewedAtUtc { get; set; }

        /// <summary>
        /// Ghi chú xử lý từ admin.
        /// </summary>
        public string? ReviewNote { get; set; }

        /// <summary>
        /// Danh mục dịch vụ thợ có thể nhận.
        /// </summary>
        public ICollection<WorkerCategory> WorkerCategories { get; set; } = new List<WorkerCategory>();

        /// <summary>
        /// Tài liệu đăng ký (bằng cấp, CCCD, CV…).
        /// </summary>
        public ICollection<WorkerRegistrationDocument> RegistrationDocuments { get; set; } = new List<WorkerRegistrationDocument>();

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }
    }
}

