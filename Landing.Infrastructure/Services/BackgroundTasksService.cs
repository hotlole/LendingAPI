using Hangfire;
using Landing.Core.Models.Users;
using Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Landing.Infrastructure.Services
{
    public class BackgroundTasksService : BackgroundService
    {
        private readonly ILogger<BackgroundTasksService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundTasksService(ILogger<BackgroundTasksService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Фоновый сервис запущен.");

            // Запускаем фоновые задачи через Hangfire
            RecurringJob.AddOrUpdate("DeleteOldFiles", () => DeleteOldFilesAsync(), Cron.HourInterval(6));
            RecurringJob.AddOrUpdate("AwardBirthdayPoints", () => AwardBirthdayPointsAsync(), Cron.Daily);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Фоновый сервис работает...");
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }

            _logger.LogInformation("Фоновый сервис остановлен.");
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task DeleteOldFilesAsync()
        {
            _logger.LogInformation("Удаление старых файлов...");

            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (Directory.Exists(folderPath))
                {
                    var files = Directory.GetFiles(folderPath);
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.CreationTime < DateTime.Now.AddDays(-30)) // Старше 30 дней
                        {
                            fileInfo.Delete();
                            _logger.LogInformation($"Удалён файл: {fileInfo.Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при удалении файлов: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task AwardBirthdayPointsAsync()
        {
            _logger.LogInformation("Начисление баллов на день рождения...");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var today = DateTime.Today;
                var users = await context.Users
                    .Where(u => u.BirthDate.HasValue && u.BirthDate.Value.Date == today)
                    .ToListAsync();

                foreach (var user in users)
                {
                    var transaction = new UserPointsTransaction
                    {
                        UserId = user.Id,
                        Points = 100, 
                        Description = "Начисление баллов на день рождения",
                        CreatedAt = DateTime.UtcNow
                    };

                    context.UserPointsTransactions.Add(transaction);
                    _logger.LogInformation($"Начислены баллы пользователю {user.Id}.");
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при начислении баллов: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}
