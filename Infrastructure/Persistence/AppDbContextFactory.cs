using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        TryLoadDotEnv();

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Thiếu cấu hình chuỗi kết nối. Vui lòng thiết lập biến môi trường 'ConnectionStrings__DefaultConnection' (hoặc khai báo trong file API/.env).");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private static void TryLoadDotEnv()
    {
        var baseDir = Directory.GetCurrentDirectory();

        var candidates = new[]
        {
            Path.Combine(baseDir, ".env"),
            Path.Combine(baseDir, "API", ".env"),
            Path.Combine(baseDir, "..", "API", ".env"),
            Path.Combine(baseDir, "..", "..", "API", ".env"),
        };

        foreach (var path in candidates)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            DotNetEnv.Env.Load(path);
            return;
        }
    }
}
