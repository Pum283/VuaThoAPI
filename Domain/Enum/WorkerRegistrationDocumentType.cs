namespace Domain.Enum;

/// <summary>
/// Loại tài liệu đính kèm khi đăng ký thợ.
/// </summary>
public enum WorkerRegistrationDocumentType
{
    /// <summary>Bằng cấp / chứng chỉ.</summary>
    Diploma = 1,

    /// <summary>CCCD / giấy tờ định danh.</summary>
    IdentityCard = 2,

    /// <summary>Giấy tờ khác.</summary>
    Other = 3,

    /// <summary>CV (thợ công ty).</summary>
    CurriculumVitae = 4,
}
