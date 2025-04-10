using Landing.Core.Models.Users;

namespace Landing.Application.Interfaces
{
    public interface IUserTransactionService
    {
        Task AddPointsAsync(int userId, int points, string description);
        Task SubtractPointsAsync(int userId, int points, string description);
        Task<IList<UserPointsTransaction>> GetUserTransactionsAsync(int userId);
    }
}
