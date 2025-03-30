using Landing.Core.Models;

namespace Landing.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string userId);
        Task UpdateAsync(User user);
    }
}
