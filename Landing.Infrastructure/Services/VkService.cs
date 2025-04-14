using Landing.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

public class VkService
{
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private readonly string _version;
    private readonly string _groupId;

    public VkService(IConfiguration config, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _token = config["Vk:ServiceToken"]!; 
        _version = "5.199";
        _groupId = config["Vk:GroupId"]!;
    }

    public async Task<List<VkPostDto>> GetGroupPostsAsync(int count = 10)
    {
        var url = $"https://api.vk.com/method/wall.get?owner_id=-{_groupId}&count={count}&access_token={_token}&v={_version}";
        var responseJson = await _httpClient.GetFromJsonAsync<JsonElement>(url);

        if (responseJson.TryGetProperty("error", out var error))
            throw new Exception($"VK API error: {error.GetProperty("error_msg").GetString()}");

        if (!responseJson.TryGetProperty("response", out var response) ||
            !response.TryGetProperty("items", out var items))
        {
            throw new Exception("VK API: отсутствует 'response.items'. Ответ: " + responseJson.ToString());
        }

        var posts = new List<VkPostDto>();

        foreach (var item in items.EnumerateArray())
        {
            var text = item.GetProperty("text").GetString();
            var unixDate = item.GetProperty("date").GetInt32();
            var date = DateTimeOffset.FromUnixTimeSeconds(unixDate).UtcDateTime;

            string? imageUrl = null;
            if (item.TryGetProperty("attachments", out var attachments))
            {
                imageUrl = attachments.EnumerateArray()
                    .Where(a => a.GetProperty("type").GetString() == "photo")
                    .SelectMany(a => a.GetProperty("photo").GetProperty("sizes").EnumerateArray())
                    .OrderByDescending(s => s.GetProperty("type").GetString())
                    .Select(s => s.GetProperty("url").GetString())
                    .FirstOrDefault();
            }

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
