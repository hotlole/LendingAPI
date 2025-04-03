using Landing.Application.Services;
using Landing.Core.Models;
using Landing.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

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
        private readonly IMapper _mapper;

        public NewsController(NewsService newsService, IWebHostEnvironment environment, IMapper mapper)
        {
            _newsService = newsService;
            _environment = environment;
            _mapper = mapper;
        }
        /// <summary>
        /// Получить список всех новостей с поддержкой поиска, сортировки и пагинации.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? sortBy, [FromQuery] string? sortOrder, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var newsQuery = _newsService.GetAllNewsAsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                newsQuery = newsQuery.Where(n => n.Title.Contains(search) || n.Description.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var sortingOrder = sortOrder?.ToLower() == "desc" ? "descending" : "ascending";
                try
                {
                    newsQuery = newsQuery.OrderBy($"{sortBy} {sortingOrder}");
                }
                catch (Exception)
                {
                    return BadRequest("Неверное поле для сортировки.");
                }
            }

            var totalItems = await newsQuery.CountAsync();
            var news = await newsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var newsDtos = _mapper.Map<IEnumerable<NewsDto>>(news);
            return Ok(new { totalItems, page, pageSize, items = newsDtos });
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

            var newsDto = _mapper.Map<NewsDto>(newsItem);
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

            var news = _mapper.Map<News>(dto);
            news.ImageUrl = dto.ImageFile != null ? await SaveImageAsync(dto.ImageFile) : null;
            news.PublishedAt = DateTime.UtcNow;

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

            _mapper.Map(dto, existingNews);

            if (dto.ImageFile != null)
            {
                existingNews.ImageUrl = await SaveImageAsync(dto.ImageFile);
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
