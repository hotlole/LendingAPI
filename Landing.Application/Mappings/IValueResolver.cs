using AutoMapper;
using Landing.Application.DTOs.News;
using Landing.Core.Models.News;
using Microsoft.AspNetCore.Http;

namespace Landing.Application.Mappings
{
    public class ImageUrlResolver : IValueResolver<News, NewsDto, string?>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Resolve(News source, NewsDto destination, string? destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ImageUrl))
                return null;

            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null || source.ImageUrl.StartsWith("http"))
                return source.ImageUrl;

            return $"{request.Scheme}://{request.Host}{source.ImageUrl}";
        }
    }
}
