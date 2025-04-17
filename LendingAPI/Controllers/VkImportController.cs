using Landing.Core.Enums;
using Landing.Core.Models;
using Landing.Core.Models.News;
using Landing.Infrastructure.Data;
using Landing.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
        // <summary>
        /// Импортирует последние посты из группы VK.
        /// </summary>
        /// <remarks>
        /// Импортирует до 5 последних постов из VK-группы, исключая уже импортированные посты с таким же заголовком.
        /// Также загружает изображения, масштабируя их в соответствии с заданным размером.
        /// </remarks>
        /// <param name="preferredSize">
        /// Желаемый размер изображений:
        /// 0 = Original (Оригинальный размер), 
        /// 1 = 512x380, 
        /// 2 = 256x190.
        /// </param>
        /// <response code="200">Успешно импортировано. Возвращает количество импортированных постов.</response>
        /// <response code="401">Неавторизованный доступ.</response>
        /// <response code="403">Доступ запрещен (нужна роль администратора).</response>
        /// <response code="500">Внутренняя ошибка сервера при обработке запроса.</response>
        [HttpPost("import")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ImportVkPosts([FromQuery] ImageSize preferredSize = ImageSize.Size_512x380)
        {
            var posts = await _vkService.GetGroupPostsAsync(count: 5);
            int imported = 0;

            foreach (var post in posts)
            {
                if (string.IsNullOrWhiteSpace(post.Text))
                    continue;

                if (_context.News.Any(n => n.Title == post.Text))
                    continue;

                var title = post.Text.Length > 100 ? post.Text[..100] + "..." : post.Text;
                string? savedImagePath = null;
                string? fileBase = null;

                // Сохраняем главное изображение
                if (!string.IsNullOrWhiteSpace(post.ImageUrl))
                {
                    try
                    {
                        var dimensions = preferredSize.GetDimensions();
                        var suffix = preferredSize.ToFileSuffix();
                        fileBase = $"vk_{post.PublishedAt:yyyyMMdd_HHmmss}";
                        var path = Path.Combine("wwwroot", "images/news");
                        Directory.CreateDirectory(path);
                        var filePath = Path.Combine(path, $"{fileBase}_{suffix}.jpg");

                        using var httpClient = new HttpClient();
                        var imageBytes = await httpClient.GetByteArrayAsync(post.ImageUrl);
                        using var image = Image.Load(imageBytes);

                        if (dimensions.HasValue)
                            image.Mutate(x => x.Resize(dimensions.Value.width, dimensions.Value.height));

                        await image.SaveAsJpegAsync(filePath);
                        savedImagePath = Path.Combine("images/news", $"{fileBase}_{suffix}.jpg").Replace("\\", "/");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Ошибка при сохранении изображения: {Error}", ex.Message);
                    }
                }

                var news = new News
                {
                    Title = title,
                    Description = post.Text,
                    Content = post.Text,
                    PublishedAt = post.PublishedAt,
                    ImageUrl = savedImagePath,
                    VideoUrl = post.VideoUrl,
                    VideoPreviewUrl = post.VideoPreviewUrl,
                    ExternalLink = post.ExternalLink,
                    LinkTitle = post.LinkTitle,
                    AdditionalImages = new List<NewsImage>()
                };

                // Сохраняем дополнительные изображения
                if (post.AdditionalImages != null && post.AdditionalImages.Count > 1)
                {
                    foreach (var imageUrl in post.AdditionalImages.Skip(1)) // первую мы уже сохранили
                    {
                        news.AdditionalImages.Add(new NewsImage
                        {
                            Url = imageUrl
                        });
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
