using Landing.Application.DTOs;
using Landing.Application.Interfaces;
using Landing.Core.Models;
using Landing.Core.Models.Events;
using Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class EventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;

    public EventService(IEventRepository eventRepository, IUserRepository userRepository, ApplicationDbContext context)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<IEnumerable<Event>> GetAllAsync() => await _eventRepository.GetAllAsync();

    public async Task<Event?> GetByIdAsync(int id) => await _eventRepository.GetByIdAsync(id);

    private bool IsValidCoordinate(decimal? coordinate)
    {
        return coordinate.HasValue && coordinate >= -90 && coordinate <= 90;
    }

    public async Task AddAsync(EventDto eventDto)
    {
        Event eventItem;
        switch (eventDto.Type)
        {
            case EventType.Regular:
                eventItem = new Event
                {
                    Title = eventDto.Title,
                    Description = eventDto.Description,
                    Date = eventDto.Date
                };
                break;
            case EventType.Curated:
                eventItem = new CuratedEvent
                {
                    Title = eventDto.Title,
                    Description = eventDto.Description,
                    Date = eventDto.Date,
                    Curators = new List<User>()
                };
                break;
            case EventType.Offline:
                if (!IsValidCoordinate(eventDto.Latitude) || !IsValidCoordinate(eventDto.Longitude))
                    throw new ArgumentException("Координаты должны быть в диапазоне от -90 до 90.");
                eventItem = new OfflineEvent
                {
                    Title = eventDto.Title,
                    Description = eventDto.Description,
                    Date = eventDto.Date,
                    Latitude = eventDto.Latitude ?? 0,
                    Longitude = eventDto.Longitude ?? 0,
                    Address = eventDto.Address,
                    CustomHtmlTemplate = eventDto.CustomHtmlTemplate ?? string.Empty
                };
                break;
            default:
                throw new ArgumentException("Некорректный тип мероприятия.");
        }
        await _eventRepository.AddAsync(eventItem);
    }

    public async Task UpdateAsync(Event eventItem) => await _eventRepository.UpdateAsync(eventItem);

    public async Task DeleteAsync(int id) => await _eventRepository.DeleteAsync(id);

    public async Task<bool> RegisterForEventAsync(int eventId, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsEmailConfirmed)
            throw new InvalidOperationException("Пользователь не найден или почта не подтверждена.");

        var eventItem = await _eventRepository.GetByIdAsync(eventId) as RegularEvent;
        if (eventItem == null)
            throw new InvalidOperationException("Мероприятие не найдено или не является обычным.");

        if (!eventItem.Participants.Any(p => p.Id == user.Id))
        {
            eventItem.Participants.Add(user);
            await _eventRepository.UpdateAsync(eventItem);
            return true;
        }

        return false;
    }

    public async Task ConfirmAttendanceAsync(int userId, int eventId, int points)
    {
        var attendance = await _context.EventAttendances
            .FirstOrDefaultAsync(ea => ea.UserId == userId && ea.EventId == eventId);

        if (attendance == null)
            throw new Exception("Запись не найдена");

        if (attendance.IsConfirmed)
            throw new Exception("Явка уже подтверждена");

        attendance.IsConfirmed = true;
        attendance.PointsAwarded = points;
        attendance.ConfirmedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task AssignCuratorAsync(int eventId, int userId)
    {
        var eventItem = await _eventRepository.GetByIdAsync(eventId) as CuratedEvent;
        if (eventItem == null)
            throw new Exception("Мероприятие не найдено или не является курируемым.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("Пользователь не найден.");

        if (!eventItem.Curators.Any(c => c.Id == user.Id))
        {
            eventItem.Curators.Add(user);
            await _eventRepository.UpdateAsync(eventItem);
        }
    }

    public async Task<string?> GetHtmlTemplateAsync(int eventId)
    {
        var eventItem = await _eventRepository.GetByIdAsync(eventId) as OfflineEvent;
        return eventItem?.CustomHtmlTemplate;
    }

    public async Task UpdateHtmlTemplateAsync(int eventId, string htmlTemplate)
    {
        var eventItem = await _eventRepository.GetByIdAsync(eventId) as OfflineEvent;
        if (eventItem == null)
            throw new Exception("Мероприятие не найдено.");

        eventItem.CustomHtmlTemplate = htmlTemplate;
        await _eventRepository.UpdateAsync(eventItem);
    }
}
