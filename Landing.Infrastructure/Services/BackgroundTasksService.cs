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

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DeleteOldFilesAsync();
                    await AwardBirthdayPointsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при выполнении фоновой задачи: {ex.Message}");
                }

                // с таким то интервалом 
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }

            _logger.LogInformation("Фоновый сервис остановлен.");
        }

        private async Task DeleteOldFilesAsync()
        {
            //удаления мусорных файлов
            _logger.LogInformation("Удаление старых файлов...");
            await Task.CompletedTask;
        }

        private async Task AwardBirthdayPointsAsync()
        {
            //начисление баллов на день рождения
            _logger.LogInformation("Начисление баллов на день рождения...");
            await Task.CompletedTask;
        }
    }
}
