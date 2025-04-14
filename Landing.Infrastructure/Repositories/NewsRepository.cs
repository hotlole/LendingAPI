using Landing.Application.Interfaces;
using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Landing.Infrastructure.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public NewsRepository(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IQueryable<News> GetAll() => _context.News.AsQueryable();

        public async Task<News?> GetByIdAsync(int id) => await _context.News.FindAsync(id);

        public async Task AddAsync(News news)
        {
            _context.News.Add(news);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(News news)
        {
            _context.News.Update(news);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                if (!string.IsNullOrEmpty(news.ImageUrl))
                {
                    var fullPath = Path.Combine(_env.WebRootPath, news.ImageUrl);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }

                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
        }
    }
}
