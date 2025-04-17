using Landing.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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

            var post = new VkPostDto
            {
                Text = text ?? "",
                PublishedAt = date
            };

            if (item.TryGetProperty("attachments", out var attachments))
            {
                foreach (var attachment in attachments.EnumerateArray())
                {
                    var type = attachment.GetProperty("type").GetString();

                    switch (type)
                    {
                        case "photo":
                            var sizes = attachment.GetProperty("photo").GetProperty("sizes");
                            var bestSize = sizes.EnumerateArray()
                                .OrderByDescending(s => s.GetProperty("width").GetInt32())
                                .FirstOrDefault();

                            if (bestSize.ValueKind != JsonValueKind.Undefined)
                            {
                                var imageUrl = bestSize.GetProperty("url").GetString();
                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    if (post.ImageUrl == null)
                                        post.ImageUrl = imageUrl;

                                    post.AdditionalImages.Add(imageUrl);
                                }
                            }
                            break;

                        case "video":
                            if (attachment.TryGetProperty("video", out var video))
                            {
                                post.VideoPreviewUrl = video.GetProperty("photo_800").GetString() ??
                                                       video.GetProperty("photo_640").GetString();

                                post.VideoUrl = video.GetProperty("player").GetString();
                            }
                            break;

                        case "link":
                            if (attachment.TryGetProperty("link", out var link))
                            {
                                post.ExternalLink = link.GetProperty("url").GetString();
                                post.LinkTitle = link.GetProperty("title").GetString();
                            }
                            break;
                    }
                }
            }

            posts.Add(post);
        }

        return posts;
    }

    public void DeleteNewsImages(string fileBase, string folder)
    {
        var sizes = new[] { "512x380", "256x190", "original" };

        foreach (var size in sizes)
        {
            var path = Path.Combine("wwwroot", folder, $"{fileBase}_{size}.jpg");
            if (File.Exists(path)) File.Delete(path);
        }
    }

}
