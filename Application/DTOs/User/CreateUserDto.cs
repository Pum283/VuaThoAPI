namespace Application.DTOs.User
{
    /// <summary>
    /// Dữ liệu tạo mới người dùng.
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// Số điện thoại đăng nhập.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Họ và tên hiển thị.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email (nếu có).
        /// </summary>
        public string? Email { get; set; }
    }
}

