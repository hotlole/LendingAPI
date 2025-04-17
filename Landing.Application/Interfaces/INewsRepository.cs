using Landing.Core.Models.News;

namespace Landing.Application.Interfaces
{
    public interface INewsRepository
    {
        IQueryable<News> GetAll(); 
        Task<News?> GetByIdAsync(int id);
        Task AddAsync(News news);
        Task UpdateAsync(News news);
        Task DeleteAsync(int id);
    }
}
