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

        public async Task<IEnumerable<News>> GetAllAsync() => await _newsRepository.GetAllAsync();

        public async Task<News?> GetByIdAsync(int id) => await _newsRepository.GetByIdAsync(id);

        public async Task AddAsync(News news) => await _newsRepository.AddAsync(news);

        public async Task UpdateAsync(News news) => await _newsRepository.UpdateAsync(news);

        public async Task DeleteAsync(int id) => await _newsRepository.DeleteAsync(id);
    }
}
