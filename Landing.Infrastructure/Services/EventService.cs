using Landing.Application.Interfaces;
using Landing.Core.Models;

namespace Landing.Application.Services
{
    public class EventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<Event>> GetAllAsync() => await _eventRepository.GetAllAsync();

        public async Task<Event?> GetByIdAsync(int id) => await _eventRepository.GetByIdAsync(id);

        public async Task AddAsync(Event eventItem) => await _eventRepository.AddAsync(eventItem);

        public async Task UpdateAsync(Event eventItem) => await _eventRepository.UpdateAsync(eventItem);

        public async Task DeleteAsync(int id) => await _eventRepository.DeleteAsync(id);
    }
}
