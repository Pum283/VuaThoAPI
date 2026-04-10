namespace Domain.Enum
{
    /// <summary>
    /// Trạng thái hồ sơ thợ.
    /// </summary>
    public enum WorkerStatus
    {
        /// <summary>
        /// Chờ duyệt.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Đã được duyệt.
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Bị từ chối.
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Chưa kích hoạt/Tạm ngưng.
        /// </summary>
        Inactive = 4
    }
}

