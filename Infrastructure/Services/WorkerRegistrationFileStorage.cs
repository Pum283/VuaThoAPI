using Application.Interfaces.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public sealed class WorkerRegistrationFileStorage : IWorkerRegistrationFileStorage
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public WorkerRegistrationFileStorage(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    public async Task<string> SaveAsync(Guid workerProfileId, string originalFileName, Stream content, CancellationToken cancellationToken = default)
    {
        var blobPath = await TrySaveToAzureBlobAsync(workerProfileId, originalFileName, content, cancellationToken);
        if (!string.IsNullOrWhiteSpace(blobPath))
        {
            return blobPath;
        }

        return await SaveToLocalWwwRootAsync(workerProfileId, originalFileName, content, cancellationToken);
    }

    private async Task<string?> TrySaveToAzureBlobAsync(Guid workerProfileId, string originalFileName, Stream content, CancellationToken cancellationToken)
    {
        var connectionString = _configuration["AZURE_BLOB_STORAGE"] ?? _configuration["AzureBlobStorage:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return null;
        }

        var containerName = _configuration["AZURE_BLOB_CONTAINER"] ??
                            _configuration["AzureBlobStorage:ContainerName"] ??
                            "worker-registrations";

        var ext = ResolveSafeExtension(originalFileName);
        var blobName = $"{workerProfileId}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{ext}";

        if (content.CanSeek)
        {
            content.Position = 0;
        }

        var blobContainerClient = new BlobContainerClient(connectionString, containerName);
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(
            content,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = ResolveContentType(ext)
                }
            },
            cancellationToken);

        return blobClient.Uri.ToString();
    }

    private async Task<string> SaveToLocalWwwRootAsync(Guid workerProfileId, string originalFileName, Stream content, CancellationToken cancellationToken)
    {
        var webRoot = string.IsNullOrEmpty(_env.WebRootPath)
            ? Path.Combine(_env.ContentRootPath, "wwwroot")
            : _env.WebRootPath;

        Directory.CreateDirectory(webRoot);

        var safeExt = ResolveSafeExtension(originalFileName);
        var uniqueName = $"{Guid.NewGuid():N}{safeExt}";
        var relativeDir = Path.Combine("uploads", "worker-registrations", workerProfileId.ToString());
        var absoluteDir = Path.Combine(webRoot, relativeDir);
        Directory.CreateDirectory(absoluteDir);

        var absolutePath = Path.Combine(absoluteDir, uniqueName);
        await using (var fs = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true))
        {
            await content.CopyToAsync(fs, cancellationToken);
        }

        return Path.Combine(relativeDir, uniqueName).Replace('\\', '/');
    }

    private static string ResolveSafeExtension(string originalFileName)
    {
        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(ext))
        {
            return ".bin";
        }

        return ext.Length > 10 ? ext[..10] : ext;
    }

    private static string ResolveContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
