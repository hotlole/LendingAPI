using Landing.Core.Models;
using Landing.Core.Models.Users;

namespace Landing.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        RefreshToken GenerateRefreshToken();
        Task<bool> RevokeRefreshToken(string token);
        Task<RefreshToken?> GetRefreshToken(string token);

    }
}
