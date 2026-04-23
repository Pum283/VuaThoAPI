namespace Application.DTOs.User
{
    /// <summary>
    /// Thông tin người dùng trả về cho client.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// Định danh người dùng.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Số điện thoại.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Họ và tên.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email (nếu có).
        /// </summary>
        public string? Email { get; set; }
    }
}

