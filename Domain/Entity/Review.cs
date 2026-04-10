using Domain.Common;

namespace Domain.Entity
{
    /// <summary>
    /// Đánh giá của khách hàng cho một booking (chỉ tạo khi booking hoàn thành).
    /// </summary>
    public class Review : BaseAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Booking được đánh giá.
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Booking được đánh giá.
        /// </summary>
        public Booking? Booking { get; set; }

        /// <summary>
        /// Số sao (1-5).
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Nội dung đánh giá (tiếng Việt tự nhiên).
        /// </summary>
        public string? Comment { get; set; }

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }
    }
}

