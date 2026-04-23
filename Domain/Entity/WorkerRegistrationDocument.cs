using Domain.Common;
using Domain.Enum;

namespace Domain.Entity;

/// <summary>
/// File đính kèm hồ sơ đăng ký thợ (bằng cấp, CCCD, CV…).
/// </summary>
public class WorkerRegistrationDocument : BaseAuditableEntity, ISoftDelete
{
    public Guid WorkerProfileId { get; set; }

    public WorkerProfile WorkerProfile { get; set; } = null!;

    public WorkerRegistrationDocumentType DocumentType { get; set; }

    /// <summary>Tên file gốc do người dùng chọn.</summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>Đường dẫn lưu trữ file (URL Azure Blob hoặc đường dẫn tương đối local).</summary>
    public string StoredRelativePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? DeletedBy { get; set; }
}
