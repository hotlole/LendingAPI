using Landing.Application.Services;
using Landing.Core.Models;
using LandingAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingAPI.Controllers
{
    /// <summary>
    /// Контроллер для управления новостями.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsService _newsService;
        private readonly IWebHostEnvironment _environment;

        public NewsController(NewsService newsService, IWebHostEnvironment environment)
        {
            _newsService = newsService;
            _environment = environment;
        }

        /// <summary>
        /// Получить список всех новостей.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var news = await _newsService.GetAllNewsAsync();

            var newsDtos = news.Select(n => new NewsDto
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                ImageUrl = n.ImageUrl,
                PublishedAt = n.PublishedAt
            });

            return Ok(newsDtos);
        }

        /// <summary>
        /// Получить новость по ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var newsItem = await _newsService.GetNewsByIdAsync(id);
            if (newsItem == null)
                return NotFound();

            var newsDto = new NewsDto
            {
                Id = newsItem.Id,
                Title = newsItem.Title,
                Description = newsItem.Description,
                ImageUrl = newsItem.ImageUrl,
                PublishedAt = newsItem.PublishedAt
            };

            return Ok(newsDto);
        }

        /// <summary>
        /// Создать новость.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateNewsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? imagePath = null;
            if (dto.ImageFile != null)
            {
                imagePath = await SaveImageAsync(dto.ImageFile);
            }

            var news = new News
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = imagePath,
                PublishedAt = DateTime.UtcNow
            };

            await _newsService.AddNewsAsync(news);
            return CreatedAtAction(nameof(GetById), new { id = news.Id }, news);
        }

        /// <summary>
        /// Обновить новость.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateNewsDto dto)
        {
            var existingNews = await _newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Title))
                existingNews.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                existingNews.Description = dto.Description;

            if (dto.ImageFile != null)
            {
                string imagePath = await SaveImageAsync(dto.ImageFile);
                existingNews.ImageUrl = imagePath;
            }

            await _newsService.UpdateNewsAsync(existingNews);
            return NoContent();
        }

        /// <summary>
        /// Удалить новость.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _newsService.DeleteNewsAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Сохранить изображение.
        /// </summary>
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}";
        }
    }
}
