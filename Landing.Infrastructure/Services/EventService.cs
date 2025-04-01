using Landing.Application.Interfaces;
using Landing.Core.Models;
using Landing.Infrastructure.Repositories;

public class EventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;

    public EventService(IEventRepository eventRepository, IUserRepository userRepository)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<Event>> GetAllAsync() => await _eventRepository.GetAllAsync();
    public async Task<Event?> GetByIdAsync(int id) => await _eventRepository.GetByIdAsync(id);
    public async Task AddAsync(Event eventItem) => await _eventRepository.AddAsync(eventItem);
    public async Task UpdateAsync(Event eventItem) => await _eventRepository.UpdateAsync(eventItem);
    public async Task DeleteAsync(int id) => await _eventRepository.DeleteAsync(id);

    public async Task<bool> RegisterForEventAsync(int eventId, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsEmailConfirmed)
            throw new InvalidOperationException("Пользователь не найден или почта не подтверждена.");

        var eventItem = await _eventRepository.GetByIdAsync(eventId);
        if (eventItem == null)
            throw new InvalidOperationException("Мероприятие не найдено.");

        if (!eventItem.Participants.Any(p => p.Id == user.Id))
        {
            eventItem.Participants.Add(user);
            await _eventRepository.UpdateAsync(eventItem);
            return true;
        }

        return false;
    }
}
