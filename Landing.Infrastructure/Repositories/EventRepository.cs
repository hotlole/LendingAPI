using Landing.Application.Interfaces;
using Landing.Core.Models.Events;
using Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Landing.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EventRepository(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<Event> CreateAsync(Event eventItem)
        {
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
            return eventItem;
        }

        public async Task<Event> UpdateAsync(Event eventItem)
        {
            _context.Events.Update(eventItem);
            await _context.SaveChangesAsync();
            return eventItem;
        }

        public async Task AddAsync(Event eventEntity)
        {
            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();
        }

        public async Task  DeleteAsync(int id)
        {
            var entity = await _context.Events.FindAsync(id);
            if (entity == null) return;

            if (!string.IsNullOrEmpty(entity.ImagePath))
            {
                var fullPath = Path.Combine(_env.WebRootPath, entity.ImagePath);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }

            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
