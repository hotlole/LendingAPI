using System.Text.Json;

namespace Landing.Application.DTOs.Vk
{
    public class VkWallGetResponse
    {
        public VkWallGetInnerResponse Response { get; set; }
    }

    public class VkWallGetInnerResponse
    {
        public List<JsonElement> Items { get; set; }
    }

}
