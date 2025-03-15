using Landing.Application.Services;
using Landing.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LandingAPI.Controllers
{
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

        // Получение всех новостей
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var news = await _newsService.GetAllNewsAsync();
            return Ok(news);
        }

        // Получение новости по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var newsItem = await _newsService.GetNewsByIdAsync(id);
            if (newsItem == null)
                return NotFound();

            return Ok(newsItem);
        }

        // Создание новости с изображением
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] News news, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (imageFile != null)
            {
                string imagePath = await SaveImageAsync(imageFile);
                news.ImageUrl = imagePath;
            }

            await _newsService.AddNewsAsync(news);
            return CreatedAtAction(nameof(GetById), new { id = news.Id }, news);
        }

        // Обновление новости с изображением
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] News news, IFormFile? imageFile)
        {
            if (id != news.Id)
                return BadRequest("ID в запросе не совпадает с ID объекта.");

            if (imageFile != null)
            {
                string imagePath = await SaveImageAsync(imageFile);
                news.ImageUrl = imagePath;
            }

            await _newsService.UpdateNewsAsync(news);
            return NoContent();
        }

        // Удаление новости
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _newsService.DeleteNewsAsync(id);
            return NoContent();
        }

        // Метод для сохранения изображения
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
