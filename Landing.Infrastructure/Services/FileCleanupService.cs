using Landing.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Landing.Infrastructure.Services;

public class FileCleanupService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FileCleanupService> _logger;

    public FileCleanupService(ApplicationDbContext context, IWebHostEnvironment env, ILogger<FileCleanupService> logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }
    public async Task<int> CleanupOrphanedFilesAsync(int olderThanDays = 3)
    {
        int deletedCount = 0;

        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

        if (!Directory.Exists(uploadsFolder))
            return deletedCount;

        var usedPaths = new HashSet<string>(
            (await _context.News.Select(n => n.ImageUrl).ToListAsync())
            .Concat(await _context.Events.Select(e => e.ImagePath).ToListAsync())
            .Where(p => !string.IsNullOrEmpty(p))
        );

        var allFiles = Directory.GetFiles(uploadsFolder, "*", SearchOption.AllDirectories);

        foreach (var filePath in allFiles)
        {
            string relativePath = Path.GetRelativePath(_env.WebRootPath, filePath).Replace("\\", "/");

            if (!usedPaths.Contains(relativePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.CreationTimeUtc < DateTime.UtcNow.AddDays(-olderThanDays))
                {
                    try
                    {
                        fileInfo.Delete();
                        _logger.LogInformation($"Удалён неиспользуемый файл: {relativePath}");
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Ошибка при удалении файла {relativePath}");
                    }
                }
            }
        }

        return deletedCount;
    }


}
