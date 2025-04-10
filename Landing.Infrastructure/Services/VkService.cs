using Landing.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;


namespace Landing.Infrastructure.Services
{
    public class VkService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private readonly string _version;

        public VkService(IConfiguration config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _token = config["Vk:Token"]!;
            _version = "5.199";
        }

        public async Task<List<VkPostDto>> GetGroupPostsAsync(string groupId, int count = 10)
        {
            var url = $"https://api.vk.com/method/wall.get?owner_id=-{groupId}&count={count}&access_token={_token}&v={_version}";
            var response = await _httpClient.GetFromJsonAsync<JsonElement>(url);

            var items = response.GetProperty("response").GetProperty("items");

            var posts = new List<VkPostDto>();

            foreach (var item in items.EnumerateArray())
            {
                var text = item.GetProperty("text").GetString();
                var unixDate = item.GetProperty("date").GetInt32();
                var date = DateTimeOffset.FromUnixTimeSeconds(unixDate).DateTime;

                string? imageUrl = item.TryGetProperty("attachments", out var attachments)
                    ? attachments.EnumerateArray()
                        .Where(a => a.GetProperty("type").GetString() == "photo")
                        .SelectMany(a => a.GetProperty("photo").GetProperty("sizes").EnumerateArray())
                        .OrderByDescending(s => s.GetProperty("type").GetString()) // x > m > s
                        .Select(s => s.GetProperty("url").GetString())
                        .FirstOrDefault()
                    : null;

                posts.Add(new VkPostDto
                {
                    Text = text ?? "",
                    PublishedAt = date,
                    ImageUrl = imageUrl
                });
            }

            return posts;
        }
    }

}
