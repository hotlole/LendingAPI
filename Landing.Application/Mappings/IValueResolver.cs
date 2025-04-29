using AutoMapper;
using Landing.Application.DTOs.News;
using Landing.Core.Models.News;
using Microsoft.AspNetCore.Http;

namespace Landing.Application.Mappings
{
    public class RelativePathResolver<TSource> : IValueResolver<TSource, object, string?>
    where TSource : class
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RelativePathResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Resolve(TSource source, object destination, string? destMember, ResolutionContext context)
        {
            var property = typeof(TSource).GetProperty("ImageUrl");
            if (property == null) return null;

            var relativePath = property.GetValue(source) as string;
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null || relativePath.StartsWith("http"))
                return relativePath;

            return $"{request.Scheme}://{request.Host}{relativePath}";
        }
    }

}
