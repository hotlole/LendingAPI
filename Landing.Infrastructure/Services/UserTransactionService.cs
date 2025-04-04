using Landing.Application.Interfaces;
using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Landing.Infrastructure.Services
{
    public class UserTransactionService : IUserTransactionService
    {
        private readonly ApplicationDbContext _context;

        public UserTransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPointsAsync(int userId, int points, string description)
        {
            await AddTransactionAsync(userId, points, description);
        }

        public async Task SubtractPointsAsync(int userId, int points, string description)
        {
            await AddTransactionAsync(userId, -points, description);
        }

        private async Task AddTransactionAsync(int userId, int points, string description)
        {
            var transaction = new UserPointsTransaction
            {
                UserId = userId,
                Points = points,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserPointsTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<UserPointsTransaction>> GetUserTransactionsAsync(int userId)
        {
            return await _context.UserPointsTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
