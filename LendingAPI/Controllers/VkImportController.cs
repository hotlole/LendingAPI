using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Landing.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> ImportVkPosts()
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

                _context.News.Add(new News
                {
                    Title = title,
                    Description = post.Text,
                    PublishedAt = post.PublishedAt,
                    ImageUrl = post.ImageUrl
                });

                _logger.LogInformation("Импортирован пост: {Title}", title);
                imported++;
            }

            await _context.SaveChangesAsync();

            return Ok(new { imported });
        }

    }

}
