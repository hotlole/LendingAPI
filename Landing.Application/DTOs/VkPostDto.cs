using AutoMapper;
using Landing.Core.Models;

namespace Landing.Application.DTOs
{
    [AutoMap(typeof(News))]
    public class VkPostDto
    {
        public string Text { get; set; } = "";
        public DateTime PublishedAt { get; set; }
        public string? ImageUrl { get; set; }
    }
}
