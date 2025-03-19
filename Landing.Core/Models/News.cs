namespace Landing.Core.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public string Description { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}
