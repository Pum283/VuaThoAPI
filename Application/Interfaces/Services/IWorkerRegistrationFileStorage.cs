namespace Application.Interfaces.Services;

/// <summary>
/// Lưu file đăng ký thợ lên ổ đĩa (wwwroot).
/// </summary>
public interface IWorkerRegistrationFileStorage
{
    /// <summary>
    /// Lưu nội dung file, trả về đường dẫn tương đối từ wwwroot (dùng cho DB và URL tĩnh).
    /// </summary>
    Task<string> SaveAsync(int workerProfileId, string originalFileName, Stream content, CancellationToken cancellationToken = default);
}
