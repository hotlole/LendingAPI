using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Landing.Application.Mappings
{
    public class RelativePathResolver<TSource> : IValueResolver<TSource, object, string?>
        where TSource : class
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _propertyName;

        public RelativePathResolver(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor, "ImagePath") { }


        public RelativePathResolver(IHttpContextAccessor httpContextAccessor, string propertyName)
        {
            _httpContextAccessor = httpContextAccessor;
            _propertyName = propertyName;
        }

        public string? Resolve(TSource source, object destination, string? destMember, ResolutionContext context)
        {
            if (source == null) return null;

            var property = typeof(TSource).GetProperty(_propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null || property.PropertyType != typeof(string))
                return null;

            var relativePath = property.GetValue(source) as string;
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            if (relativePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return relativePath;

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                return relativePath;

            return $"{request.Scheme}://{request.Host}{relativePath}";


        }
    }
}
