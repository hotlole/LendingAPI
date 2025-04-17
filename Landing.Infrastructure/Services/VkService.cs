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
        {
            var msg = error.TryGetProperty("error_msg", out var errorMsg)
                ? errorMsg.GetString()
                : "Неизвестная ошибка VK API";

            throw new Exception($"VK API error: {msg}");
        }

        if (!responseJson.TryGetProperty("response", out var response) ||
            !response.TryGetProperty("items", out var items))
        {
            throw new Exception("VK API: отсутствует 'response.items'. Ответ: " + responseJson.ToString());
        }

        var posts = new List<VkPostDto>();

        foreach (var item in items.EnumerateArray())
        {
            var text = item.TryGetProperty("text", out var textProp) ? textProp.GetString() : string.Empty;
            var unixDate = item.TryGetProperty("date", out var dateProp) ? dateProp.GetInt32() : 0;
            var date = DateTimeOffset.FromUnixTimeSeconds(unixDate).UtcDateTime;

            var post = new VkPostDto
            {
                Text = text ?? "",
                PublishedAt = date
            };

            if (item.TryGetProperty("attachments", out var attachments))
            {
                // 📝 ЛОГИРУЕМ JSON вложений
                var postId = item.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0;
                var logDir = Path.Combine("vk_logs");
                Directory.CreateDirectory(logDir);
                var logPath = Path.Combine(logDir, $"vk_attachments_{postId}.json");
                File.WriteAllText(logPath, attachments.ToString());

                foreach (var attachment in attachments.EnumerateArray())
                {
                    if (!attachment.TryGetProperty("type", out var typeProp)) continue;
                    var type = typeProp.GetString();

                    switch (type)
                    {
                        case "photo":
                            if (attachment.TryGetProperty("photo", out var photo) &&
                                photo.TryGetProperty("sizes", out var sizes))
                            {
                                var bestSize = sizes.EnumerateArray()
                                    .Where(s => s.TryGetProperty("width", out _))
                                    .OrderByDescending(s => s.GetProperty("width").GetInt32())
                                    .FirstOrDefault();

                                if (bestSize.ValueKind != JsonValueKind.Undefined &&
                                    bestSize.TryGetProperty("url", out var urlProp))
                                {
                                    var imageUrl = urlProp.GetString();
                                    if (!string.IsNullOrEmpty(imageUrl))
                                    {
                                        if (post.ImageUrl == null)
                                            post.ImageUrl = imageUrl;

                                        post.AdditionalImages.Add(imageUrl);
                                    }
                                }
                            }
                            break;

                        case "video":
                            if (attachment.TryGetProperty("video", out var video))
                            {
                                // Превью
                                post.VideoPreviewUrl = video.TryGetProperty("photo_800", out var preview800)
                                    ? preview800.GetString()
                                    : (video.TryGetProperty("photo_640", out var preview640) ? preview640.GetString() : null);

                                // Плеер, если доступен
                                if (video.TryGetProperty("player", out var playerUrl))
                                {
                                    post.VideoUrl = playerUrl.GetString();
                                }
                                else
                                {
                                    // Собираем ссылку вручную
                                    var vid = video.TryGetProperty("id", out var videoIdProp) ? videoIdProp.GetInt32() : 0;
                                    var oid = video.TryGetProperty("owner_id", out var ownerIdProp) ? ownerIdProp.GetInt32() : 0;
                                    var accessKey = video.TryGetProperty("access_key", out var accessKeyProp) ? accessKeyProp.GetString() : null;

                                    if (vid != 0 && oid != 0)
                                    {
                                        post.VideoUrl = $"https://vk.com/video{oid}_{vid}" +
                                            (string.IsNullOrEmpty(accessKey) ? "" : $"?access_key={accessKey}");
                                    }
                                }
                            }
                            break;


                        case "link":
                            if (attachment.TryGetProperty("link", out var link))
                            {
                                post.ExternalLink = link.TryGetProperty("url", out var urlProp)
                                    ? urlProp.GetString()
                                    : null;

                                post.LinkTitle = link.TryGetProperty("title", out var titleProp)
                                    ? titleProp.GetString()
                                    : null;
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
