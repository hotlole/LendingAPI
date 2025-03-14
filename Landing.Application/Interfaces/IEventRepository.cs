using Landing.Core.Models;

namespace Landing.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task AddAsync(Event eventItem);
        Task UpdateAsync(Event eventItem);
        Task DeleteAsync(int id);
    }
}
