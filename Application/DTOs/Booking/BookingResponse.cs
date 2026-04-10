using Domain.Enum;

namespace Application.DTOs.Booking
{
    /// <summary>
    /// Thông tin booking trả về cho client.
    /// </summary>
    public class BookingResponse
    {
        /// <summary>
        /// Định danh booking.
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Định danh khách hàng.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Định danh hồ sơ thợ (có thể null nếu chưa phân công).
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
        /// Địa chỉ thực hiện dịch vụ.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả yêu cầu công việc.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Tổng tiền dự kiến.
        /// </summary>
        public decimal EstimatedTotalAmount { get; set; }
    }
}

