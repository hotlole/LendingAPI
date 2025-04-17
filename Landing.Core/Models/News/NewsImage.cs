using Landing.Core.Models.News;

public class NewsImage
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;

    public int NewsId { get; set; }
    public News News { get; set; } = null!;
}
