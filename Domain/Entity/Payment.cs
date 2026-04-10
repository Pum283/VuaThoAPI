using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    /// <summary>
    /// Bản ghi thanh toán phát sinh từ booking.
    /// </summary>
    public class Payment : BaseAuditableEntity, ISoftDelete
    {
        /// <summary>
        /// Booking liên quan.
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Booking liên quan.
        /// </summary>
        public Booking? Booking { get; set; }

        /// <summary>
        /// Số tiền khách thanh toán.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Phí công ty (tính theo % theo loại thợ).
        /// </summary>
        public decimal CompanyFeeAmount { get; set; }

        /// <summary>
        /// Số tiền trả cho thợ.
        /// </summary>
        public decimal WorkerPayoutAmount { get; set; }

        /// <summary>
        /// Trạng thái thanh toán.
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; }

        /// <summary>
        /// Thời điểm thanh toán thành công (UTC) (nếu có).
        /// </summary>
        public DateTime? PaidAtUtc { get; set; }

        /// <inheritdoc />
        public bool IsDeleted { get; set; }

        /// <inheritdoc />
        public DateTime? DeletedDate { get; set; }

        /// <inheritdoc />
        public string? DeletedBy { get; set; }
    }
}

