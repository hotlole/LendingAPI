using Landing.Core.Enums;
using Landing.Core.Models;
using Landing.Core.Models.News;
using Landing.Infrastructure.Data;
using Landing.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text.Json;

namespace LendingAPI.Controllers
{
    /// <summary>
    /// Контроллер для импорта новостей из группы ВКонтакте.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class VkImportController : ControllerBase
    {
        private readonly VkService _vkService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VkImportController> _logger;
        /// <summary>
        /// Конструктор контроллера импорта из VK.
        /// </summary>
        /// <param name="vkService">Сервис для работы с VK API.</param>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="logger">Логгер.</param>
        public VkImportController(VkService vkService, ApplicationDbContext context, ILogger<VkImportController> logger)
        {
            _vkService = vkService;
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Импортирует последние посты из группы VK.
        /// </summary>
        /// <response code="200">Успешно импортировано. Возвращает количество импортированных постов.</response>
        /// <response code="401">Неавторизованный доступ.</response>
        /// <response code="403">Доступ запрещен (нужна роль администратора).</response>
        /// <response code="500">Внутренняя ошибка сервера при обработке запроса.</response>
        [HttpPost("import")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ImportVkPosts()
        {
            var posts = await _vkService.GetGroupPostsAsync(count: 5);
            int imported = 0;

            foreach (var post in posts)
            {
                if ((_context.News.Any(n => n.PublishedAt == post.PublishedAt)) && string.IsNullOrWhiteSpace(post.Text) && (post.AttachmentsRaw == null || !post.AttachmentsRaw.Value.EnumerateArray().Any()))
                {
                    continue;
                }

                if (_context.News.Any(n => n.Title == post.Text))
                    continue;

                var title = string.IsNullOrWhiteSpace(post.Text) ? "Пост без текста" :
                            post.Text.Length > 100 ? post.Text[..100] + "..." : post.Text;

                var news = new News
                {
                    Title = title,
                    Description = post.Text,
                    Content = post.Text,
                    PublishedAt = post.PublishedAt,
                    ImageUrl = null, // установим позже
                    VideoUrl = post.VideoUrl,
                    VideoPreviewUrl = post.VideoPreviewUrl,
                    ExternalLink = post.ExternalLink,
                    LinkTitle = post.LinkTitle,
                    AdditionalImages = new List<NewsImage>()
                };

                // Обработка основного изображения
                if (!string.IsNullOrWhiteSpace(post.ImageUrl))
                {
                    try
                    {
                        var fileBase = $"vk_{post.PublishedAt:yyyyMMdd_HHmmss}";
                        var path = Path.Combine("wwwroot", "uploads");
                        Directory.CreateDirectory(path);

                        // Скачиваем изображение
                        using var httpClient = new HttpClient();
                        var imageBytes = await httpClient.GetByteArrayAsync(post.ImageUrl);
                        using var image = Image.Load(imageBytes);

                        // Сохраняем оригинал
                        var originalPath = Path.Combine(path, $"{fileBase}_original.jpg");
                        await image.SaveAsJpegAsync(originalPath);

                        // Сохраняем сжатые версии
                        var sizes = new[] { (512, 380), (256, 190), (128, 95) };
                        foreach (var (width, height) in sizes)
                        {
                            var resizedPath = Path.Combine(path, $"{fileBase}_{width}x{height}.jpg");
                            image.Mutate(x => x.Resize(width, height));
                            await image.SaveAsJpegAsync(resizedPath);
                        }

                        // Добавляем пути к изображениям в NewsImage
                        string folderPath = Path.Combine("uploads", "news", post.PublishedAt.ToString("yyyy/MM/dd"));
                        Directory.CreateDirectory(Path.Combine("wwwroot", folderPath));
                        var originalImagePath = Path.Combine(folderPath, $"{fileBase}_original.jpg");

                        news.ImageUrl = $"https://localhost:7243/{originalImagePath.Replace("\\", "/")}";
                        foreach (var (width, height) in sizes)
                        {
                            news.AdditionalImages.Add(new NewsImage
                            {
                                Url = $"/uploads/{fileBase}_{width}x{height}.jpg"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Ошибка при сохранении изображения: {Error}", ex.Message);
                    }
                }

                _context.News.Add(news);
                _logger.LogInformation("Импортирован пост: {Title}", title);
                imported++;
            }

            await _context.SaveChangesAsync();

            return Ok(new { imported });
        }


    }

}
