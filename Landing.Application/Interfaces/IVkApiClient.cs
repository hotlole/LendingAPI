using Landing.Application.DTOs.Vk;
using Refit;

namespace Landing.Application.Interfaces
{
    public interface IVkApiClient
    {
        [Get("/method/wall.get")]
        Task<VkWallGetResponse> GetWallPostsAsync(
            [AliasAs("owner_id")] string ownerId,
            [AliasAs("count")] int count,
            [AliasAs("access_token")] string accessToken,
            [AliasAs("v")] string version = "5.199");
    }
}
