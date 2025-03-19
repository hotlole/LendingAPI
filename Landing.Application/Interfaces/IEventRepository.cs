using Landing.Core.Models;

namespace Landing.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateAsync(Event eventItem);
        Task<Event> UpdateAsync(Event eventItem);
        Task AddAsync(Event eventEntity);
        Task<bool> DeleteAsync(int id);
    }
}
