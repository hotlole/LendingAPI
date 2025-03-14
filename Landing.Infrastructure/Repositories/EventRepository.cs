using Landing.Application.Interfaces;
using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Landing.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllAsync() => await _context.Events.ToListAsync();

        public async Task<Event?> GetByIdAsync(int id) => await _context.Events.FindAsync(id);

        public async Task AddAsync(Event eventItem)
        {
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event eventItem)
        {
            _context.Events.Update(eventItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
