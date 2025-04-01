using Landing.Application.DTOs;
using Landing.Application.Interfaces;
using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Landing.Infrastructure.Repositories;
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
        return coordinate == null || (coordinate >= -90 && coordinate <= 90);
    }
    public async Task AddAsync(EventDto eventDto)
    {
        if (!IsValidCoordinate(eventDto.Latitude) || !IsValidCoordinate(eventDto.Longitude))
            throw new ArgumentException("Координаты должны быть в диапазоне от -90 до 90.");
        var eventItem = new Event
        {
            Title = eventDto.Title,
            Description = eventDto.Description,
            Date = eventDto.Date,
            Latitude = eventDto.Latitude,
            Longitude = eventDto.Longitude,
            Address = eventDto.Address
        };

        await _eventRepository.AddAsync(eventItem);
    }

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
    public async Task ConfirmAttendanceAsync(int userId, int eventId, int points)
    {
        var attendance = await _context.EventAttendances
            .FirstOrDefaultAsync(ea => ea.UserId == userId && ea.EventId == eventId);

        if (attendance == null)
        {
            throw new Exception("Запись не найдена");
        }

        if (attendance.IsConfirmed)
        {
            throw new Exception("Явка уже подтверждена");
        }

        attendance.IsConfirmed = true;
        attendance.PointsAwarded = points;
        attendance.ConfirmedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
    public async Task UpdateHtmlTemplateAsync(int eventId, string htmlTemplate)
    {
        var eventItem = await _eventRepository.GetByIdAsync(eventId);
        if (eventItem == null)
            throw new InvalidOperationException("Мероприятие не найдено.");

        eventItem.CustomHtmlTemplate = htmlTemplate;
        await _eventRepository.UpdateAsync(eventItem);
    }

    public async Task<string?> GetHtmlTemplateAsync(int eventId)
    {
        var eventItem = await _eventRepository.GetByIdAsync(eventId);
        return eventItem?.CustomHtmlTemplate;
    }

}
