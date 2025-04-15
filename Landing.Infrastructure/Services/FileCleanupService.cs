using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Landing.Infrastructure.Data;
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
        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsPath))
        {
            _logger.LogWarning("Папка uploads не найдена: " + uploadsPath);
            return 0;
        }

        var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
        var allFiles = Directory.GetFiles(uploadsPath, "*", SearchOption.AllDirectories);

        var usedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Получаем пути из базы
        var newsPaths = await _context.News
            .Where(n => n.ImageUrl != null)
            .Select(n => n.ImageUrl)
            .ToListAsync();

        var eventPaths = await _context.Events
            .Where(e => e.ImagePath != null)
            .Select(e => e.ImagePath)
            .ToListAsync();

        usedPaths.UnionWith(newsPaths.Select(p => Path.Combine(_env.WebRootPath, p.TrimStart('/', '\\'))));
        usedPaths.UnionWith(eventPaths.Select(p => Path.Combine(_env.WebRootPath, p.TrimStart('/', '\\'))));

        int deletedCount = 0;

        foreach (var filePath in allFiles)
        {
            if (!usedPaths.Contains(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.CreationTimeUtc < cutoffDate)
                {
                    try
                    {
                        File.Delete(filePath);
                        deletedCount++;
                        _logger.LogInformation($"Удалён неиспользуемый файл: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Ошибка при удалении файла: {filePath}");
                    }
                }
            }
        }

        return deletedCount;
    }
}
