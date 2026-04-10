namespace Application.DTOs.Auth
{
    /// <summary>
    /// Yêu cầu làm mới access token.
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// Refresh token.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }
}

