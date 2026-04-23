namespace Application.Interfaces.Services;

/// <summary>
/// Lưu file đăng ký thợ lên hệ thống lưu trữ cấu hình (Azure Blob hoặc local fallback).
/// </summary>
public interface IWorkerRegistrationFileStorage
{
    /// <summary>
    /// Lưu nội dung file, trả về đường dẫn truy cập/định danh để lưu DB.
    /// </summary>
    Task<string> SaveAsync(Guid workerProfileId, string originalFileName, Stream content, CancellationToken cancellationToken = default);
}
