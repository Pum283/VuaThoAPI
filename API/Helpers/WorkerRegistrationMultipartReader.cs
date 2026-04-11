using Application.DTOs.Auth;
using Domain.Enum;

namespace API.Helpers;

internal static class WorkerRegistrationMultipartReader
{
    public static RegisterWorkerExternalRequest ReadExternalFields(IFormCollection form)
    {
        return new RegisterWorkerExternalRequest
        {
            OtpVerificationToken = form["otpVerificationToken"].ToString(),
            PhoneNumber = form["phoneNumber"].ToString(),
            Email = form["email"].ToString(),
            Password = form["password"].ToString(),
            FullName = form["fullName"].ToString(),
        };
    }

    public static RegisterWorkerDirectRequest ReadDirectFields(IFormCollection form)
    {
        return new RegisterWorkerDirectRequest
        {
            OtpVerificationToken = form["otpVerificationToken"].ToString(),
            PhoneNumber = form["phoneNumber"].ToString(),
            Email = form["email"].ToString(),
            Password = form["password"].ToString(),
            FullName = form["fullName"].ToString(),
        };
    }

    public static IReadOnlyList<WorkerRegistrationFileUpload> ReadExternalFiles(IFormCollection form)
    {
        var list = new List<WorkerRegistrationFileUpload>();

        AddOptional(form, "diploma", WorkerRegistrationDocumentType.Diploma, list);
        AddOptional(form, "identityCard", WorkerRegistrationDocumentType.IdentityCard, list);

        var others = form.Files.GetFiles("otherDocuments");
        foreach (var file in others)
        {
            if (file.Length <= 0)
            {
                continue;
            }

            list.Add(CreateUpload(file, WorkerRegistrationDocumentType.Other));
        }

        return list;
    }

    public static IReadOnlyList<WorkerRegistrationFileUpload> ReadDirectFiles(IFormCollection form)
    {
        var list = new List<WorkerRegistrationFileUpload>();
        AddOptional(form, "cv", WorkerRegistrationDocumentType.CurriculumVitae, list);
        return list;
    }

    private static void AddOptional(
        IFormCollection form,
        string fieldName,
        WorkerRegistrationDocumentType type,
        List<WorkerRegistrationFileUpload> list)
    {
        var file = form.Files.GetFile(fieldName);
        if (file == null || file.Length <= 0)
        {
            return;
        }

        list.Add(CreateUpload(file, type));
    }

    private static WorkerRegistrationFileUpload CreateUpload(IFormFile file, WorkerRegistrationDocumentType type)
    {
        return new WorkerRegistrationFileUpload
        {
            Content = file.OpenReadStream(),
            OriginalFileName = file.FileName,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            Length = file.Length,
            DocumentType = type,
        };
    }
}
