using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    /// <summary>
    /// Đơn booking dịch vụ.
    /// </summary>
    public class Booking : BaseAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Khách hàng tạo booking.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Thợ nhận booking (có thể null khi chưa phân công).
        /// </summary>
        public int? WorkerProfileId { get; set; }

        /// <summary>
        /// Trạng thái booking.
        /// </summary>
        public BookingStatus BookingStatus { get; set; }

        /// <summary>
        /// Thời gian hẹn (UTC).
        /// </summary>
        public DateTime ScheduledAtUtc { get; set; }

        /// <summary>
        /// Địa chỉ/điểm đến thực hiện dịch vụ.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả yêu cầu công việc.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Tổng tiền dự kiến (nếu có).
        /// </summary>
        public decimal EstimatedTotalAmount { get; set; }

        /// <summary>
        /// Payment phát sinh sau khi hoàn thành (nếu có).
        /// </summary>
        public Payment? Payment { get; set; }

        /// <summary>
        /// Review sau khi hoàn thành (nếu có).
        /// </summary>
        public Review? Review { get; set; }

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }
    }
}

