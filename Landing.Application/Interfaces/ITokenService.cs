using Landing.Core.Models;

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
