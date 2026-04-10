namespace Application.DTOs.Auth
{
    /// <summary>
    /// Kết quả đăng nhập/refresh token.
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// JWT access token.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh token.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Thông tin người dùng.
        /// </summary>
        public UserInfoResponse UserInfo { get; set; } = new();
    }

    /// <summary>
    /// Thông tin user trả về cho frontend để redirect theo vai trò.
    /// </summary>
    public class UserInfoResponse
    {
        public int UserId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Tên role (lấy từ bảng Roles).
        /// </summary>
        public string Role { get; set; } = string.Empty;

        public Domain.Enum.WorkerType? WorkerType { get; set; }
        public Domain.Enum.WorkerStatus? WorkerStatus { get; set; }
    }
}

