using Landing.Core.Models.Users;

namespace Landing.Application.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        Task<User?> GetUserByRefreshToken(string refreshToken);
        Task SaveRefreshToken(User user, string refreshToken);
        Task RevokeRefreshToken(User user, string refreshToken);

    }
}
