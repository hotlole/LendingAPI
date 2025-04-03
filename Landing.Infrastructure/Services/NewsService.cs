using Landing.Application.Interfaces;
using Landing.Core.Models;

namespace Landing.Application.Services
{
    public class NewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public IQueryable<News> GetAllNewsAsQueryable()
        {
            return _newsRepository.GetAll().AsQueryable();
        }

        public async Task<News?> GetNewsByIdAsync(int id)
        {
            return await _newsRepository.GetByIdAsync(id);
        }

        public async Task AddNewsAsync(News news)
        {
            await _newsRepository.AddAsync(news);
        }

        public async Task UpdateNewsAsync(News news)
        {
            await _newsRepository.UpdateAsync(news);
        }

        public async Task DeleteNewsAsync(int id)
        {
            await _newsRepository.DeleteAsync(id);
        }
    }
}
