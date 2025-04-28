using Landing.Core.Models.Users;

namespace Landing.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId);
        Task UpdateAsync(User user);
        Task<List<User>> GetUsersByIdsAsync(List<int> userIds);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
