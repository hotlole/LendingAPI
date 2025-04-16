﻿using Landing.Core.Enums;
using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Landing.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace LendingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class VkImportController : ControllerBase
    {
        private readonly VkService _vkService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VkImportController> _logger;

        public VkImportController(VkService vkService, ApplicationDbContext context, ILogger<VkImportController> logger)
        {
            _vkService = vkService;
            _context = context;
            _logger = logger;
        }
        [HttpPost("import")]
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

                if (!string.IsNullOrWhiteSpace(post.ImageUrl))
                {
                    try
                    {
                        var dimensions = preferredSize.GetDimensions();
                        var suffix = preferredSize.ToFileSuffix();
                        var fileBase = $"vk_{post.PublishedAt:yyyyMMdd_HHmmss}";
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

                _context.News.Add(new News
                {
                    Title = title,
                    Description = post.Text,
                    PublishedAt = post.PublishedAt,
                    ImageUrl = savedImagePath
                });

                _logger.LogInformation("Импортирован пост: {Title}", title);
                imported++;
            }

            await _context.SaveChangesAsync();

            return Ok(new { imported });
        }

    }

}
