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
    /// /// <summary>
    /// Конструктор контроллера импорта из VK.
    /// </summary>
    /// <param name="vkService">Сервис для работы с VK API.</param>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="logger">Логгер.</param>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class VkImportController(VkService vkService, ApplicationDbContext context, ILogger<VkImportController> logger) : ControllerBase
    {
        private readonly VkService _vkService = vkService;
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<VkImportController> _logger = logger;
        
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
                //  Пропускать, только если нет текста И вложений
                if (string.IsNullOrWhiteSpace(post.Text) &&
                    (post.AttachmentsRaw == null || !post.AttachmentsRaw.Value.EnumerateArray().Any()))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(post.VkPostId) && _context.News.Any(n => n.VkPostId == post.VkPostId))

                {
                    _logger.LogInformation("Пропущен дубликат поста с VkPostId = {Id}", post.VkPostId);
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
                    AdditionalImages = new List<NewsImage>(),
                    VkPostId = post.VkPostId.ToString()
                };

                // Обработка основного изображения
                if (!string.IsNullOrWhiteSpace(post.ImageUrl))
                {
                    try
                    {
                        var datePath = post.PublishedAt.ToString("yyyy/MM/dd");
                        var fileBase = $"vk_{post.PublishedAt:yyyyMMdd_HHmmss}";
                        var relativeFolder = Path.Combine("uploads", "news", datePath);
                        var absoluteFolder = Path.Combine("wwwroot", relativeFolder);
                        Directory.CreateDirectory(absoluteFolder);

                        // Скачиваем изображение
                        using var httpClient = new HttpClient();
                        var imageBytes = await httpClient.GetByteArrayAsync(post.ImageUrl);
                        using var image = Image.Load(imageBytes);

                        // Сохраняем оригинал
                        var originalFileName = $"{fileBase}_original.jpg";
                        var originalPath = Path.Combine(absoluteFolder, originalFileName);
                        await image.SaveAsJpegAsync(originalPath);
                        news.ImageUrl = $"https://localhost:7243/{Path.Combine(relativeFolder, originalFileName).Replace("\\", "/")}";

                        // Сохраняем сжатые версии
                        var sizes = new[] { (512, 380), (256, 190), (128, 95) };
                        foreach (var (width, height) in sizes)
                        {
                            var resizedFileName = $"{fileBase}_{width}x{height}.jpg";
                            var resizedPath = Path.Combine(absoluteFolder, resizedFileName);

                            using var clone = image.Clone(ctx => ctx.Resize(width, height));
                            await clone.SaveAsJpegAsync(resizedPath);

                            news.AdditionalImages.Add(new NewsImage
                            {
                                Url = $"https://localhost:7243/{Path.Combine(relativeFolder, resizedFileName).Replace("\\", "/")}"
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
