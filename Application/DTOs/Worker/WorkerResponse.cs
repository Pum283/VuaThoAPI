using Domain.Enum;

namespace Application.DTOs.Worker
{
    /// <summary>
    /// Thông tin thợ trả về cho client.
    /// </summary>
    public class WorkerResponse
    {
        /// <summary>
        /// Định danh hồ sơ thợ.
        /// </summary>
        public Guid WorkerProfileId { get; set; }

        /// <summary>
        /// Định danh người dùng.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Họ và tên hiển thị.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Loại thợ.
        /// </summary>
        public WorkerType WorkerType { get; set; }

        /// <summary>
        /// Trạng thái hồ sơ thợ.
        /// </summary>
        public WorkerStatus WorkerStatus { get; set; }

        /// <summary>
        /// Vĩ độ.
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Kinh độ.
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
    }
}

