using Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services;

public sealed class WorkerRegistrationFileStorage : IWorkerRegistrationFileStorage
{
    private readonly IWebHostEnvironment _env;

    public WorkerRegistrationFileStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveAsync(int workerProfileId, string originalFileName, Stream content, CancellationToken cancellationToken = default)
    {
        var webRoot = string.IsNullOrEmpty(_env.WebRootPath)
            ? Path.Combine(_env.ContentRootPath, "wwwroot")
            : _env.WebRootPath;

        Directory.CreateDirectory(webRoot);

        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(ext))
        {
            ext = ".bin";
        }

        var safeExt = ext.Length > 10 ? ext[..10] : ext;
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
}
