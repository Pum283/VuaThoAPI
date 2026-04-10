namespace Domain.Enum
{
    /// <summary>
    /// Trạng thái booking.
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Chờ xử lý.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Đã xác nhận.
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// Đang thực hiện.
        /// </summary>
        InProgress = 3,

        /// <summary>
        /// Đã hoàn thành.
        /// </summary>
        Completed = 4,

        /// <summary>
        /// Đã huỷ.
        /// </summary>
        Cancelled = 5
    }
}

