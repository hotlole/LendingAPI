using Landing.Core.Models;

namespace Landing.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task UpdateAsync(User user);
    }
}
