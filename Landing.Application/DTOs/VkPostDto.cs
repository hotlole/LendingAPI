﻿using AutoMapper;
using Landing.Core.Models.News;

namespace Landing.Application.DTOs
{
    [AutoMap(typeof(News))]
    public class VkPostDto
    {
        public string Text { get; set; } = "";
        public DateTime PublishedAt { get; set; }
        public string? ImageUrl { get; set; }

        // Дополнительные изображения (если в посте несколько фото)
        public List<string> AdditionalImages { get; set; } = new();

        // Видео: ссылка на проигрыватель и превью
        public string? VideoUrl { get; set; }
        public string? VideoPreviewUrl { get; set; }

        // Внешняя ссылка (если в посте есть прикреплённая ссылка)
        public string? ExternalLink { get; set; }
        public string? LinkTitle { get; set; }
    }
}
