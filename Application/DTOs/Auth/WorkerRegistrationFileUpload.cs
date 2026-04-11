using Domain.Enum;

namespace Application.DTOs.Auth;

/// <summary>
/// File đính kèm luồng đăng ký thợ (stream sẽ được đọc và lưu bởi Infrastructure).
/// </summary>
public sealed class WorkerRegistrationFileUpload
{
    public required Stream Content { get; init; }

    public required string OriginalFileName { get; init; }

    public string ContentType { get; init; } = "application/octet-stream";

    public long Length { get; init; }

    public required WorkerRegistrationDocumentType DocumentType { get; init; }
}
