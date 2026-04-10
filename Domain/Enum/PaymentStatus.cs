namespace Domain.Enum
{
    /// <summary>
    /// Trạng thái thanh toán.
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Khởi tạo/chờ thanh toán.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Thanh toán thành công.
        /// </summary>
        Paid = 2,

        /// <summary>
        /// Thanh toán thất bại.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Hoàn tiền (nếu có).
        /// </summary>
        Refunded = 4
    }
}

