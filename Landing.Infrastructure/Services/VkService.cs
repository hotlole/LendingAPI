﻿using Landing.Application.DTOs;
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
        private readonly string _groupId;

        public VkService(IConfiguration config, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _token = config["Vk:Token"]!;
            _version = "5.199";
            _groupId = config["Vk:GroupId"]!;
        }

        public async Task<List<VkPostDto>> GetGroupPostsAsync(int count = 10)
        {
            var url = $"https://api.vk.com/method/wall.get?owner_id=-{_groupId}&count={count}&access_token={_token}&v={_version}";
            var responseJson = await _httpClient.GetFromJsonAsync<JsonElement>(url);

            // Проверяем, есть ли ошибка от VK API
            if (responseJson.TryGetProperty("error", out var error))
            {
                var errorMsg = error.GetProperty("error_msg").GetString();
                throw new Exception($"VK API error: {errorMsg}");
            }

            // Проверяем наличие "response" и "items"
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
                var date = DateTimeOffset.FromUnixTimeSeconds(unixDate).DateTime;

                string? imageUrl = null;

                if (item.TryGetProperty("attachments", out var attachments))
                {
                    imageUrl = attachments.EnumerateArray()
                        .Where(a => a.GetProperty("type").GetString() == "photo")
                        .SelectMany(a => a.GetProperty("photo").GetProperty("sizes").EnumerateArray())
                        .OrderByDescending(s => s.GetProperty("type").GetString()) // типы: w > z > y > r > q > p > o > x > m > s
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
}
