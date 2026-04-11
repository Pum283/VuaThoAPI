using System.Collections.Generic;
using System.Linq;
using Application.DTOs.Auth;
using Domain.Enum;

namespace Application.Common;

public static class WorkerRegistrationFileRules
{
    public const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".webp",
    };

    public static bool TryGetExtension(string originalFileName, out string extension)
    {
        extension = Path.GetExtension(originalFileName);
        return !string.IsNullOrWhiteSpace(extension);
    }

    public static bool IsAllowedExtension(string extension) => AllowedExtensions.Contains(extension);

    /// <summary>Kiểm tra danh sách file trước khi tạo tài khoản (kích thước, phần mở rộng).</summary>
    public static IReadOnlyList<string> ValidateFiles(IReadOnlyList<WorkerRegistrationFileUpload> files)
    {
        var errors = new List<string>();
        foreach (var f in files)
        {
            if (!TryGetExtension(f.OriginalFileName, out var ext) || !IsAllowedExtension(ext))
            {
                errors.Add($"File '{f.OriginalFileName}' không được hỗ trợ. Chỉ chấp nhận: PDF, JPG, JPEG, PNG, WEBP.");
            }

            if (f.Length <= 0)
            {
                errors.Add($"File '{f.OriginalFileName}' không hợp lệ (kích thước 0).");
            }

            if (f.Length > MaxFileSizeBytes)
            {
                errors.Add($"File '{f.OriginalFileName}' vượt quá 10 MB.");
            }
        }

        return errors;
    }

    public static IReadOnlyList<string> ValidateRequiredDocumentsForExternal(IReadOnlyList<WorkerRegistrationFileUpload> files)
    {
        var baseErrors = ValidateFiles(files);
        if (baseErrors.Count > 0)
        {
            return baseErrors;
        }

        if (!files.Any(x => x.DocumentType == WorkerRegistrationDocumentType.IdentityCard))
        {
            return new List<string> { "Thợ ngoài bắt buộc đính kèm file CCCD / giấy tờ định danh." };
        }

        return new List<string>();
    }

    public static IReadOnlyList<string> ValidateRequiredDocumentsForDirect(IReadOnlyList<WorkerRegistrationFileUpload> files)
    {
        var baseErrors = ValidateFiles(files);
        if (baseErrors.Count > 0)
        {
            return baseErrors;
        }

        if (!files.Any(x => x.DocumentType == WorkerRegistrationDocumentType.CurriculumVitae))
        {
            return new List<string> { "Thợ công ty bắt buộc đính kèm file CV." };
        }

        return new List<string>();
    }
}
