namespace Landing.Core.Models.News
{
    public class News
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        public string? ImageUrl { get; set; }

        // Видео
        public string? VideoUrl { get; set; }
        public string? VideoPreviewUrl { get; set; }

        // Ссылка
        public string? ExternalLink { get; set; }
        public string? LinkTitle { get; set; }

        // Навигационное свойство к дополнительным изображениям
        public List<NewsImage> AdditionalImages { get; set; } = new();
    }
}
