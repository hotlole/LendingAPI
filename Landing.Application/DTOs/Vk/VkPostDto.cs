using AutoMapper;
using Landing.Core.Models.News;
using System.Text.Json;
using NewsModel = Landing.Core.Models.News.News;

namespace Landing.Application.DTOs.Vk
{
    [AutoMap(typeof(NewsModel))]
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
        public string? VkPostId { get; set; }
        public JsonElement? AttachmentsRaw { get; set; } // сохраняем оригинальные вложения
    }
}
