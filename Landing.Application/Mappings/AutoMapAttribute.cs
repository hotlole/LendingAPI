using AutoMapper;
using System.Reflection;

namespace Landing.Application.Mappings
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetCustomAttributes<AutoMapAttribute>().Any());

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<AutoMapAttribute>()!;
                CreateMap(type, attribute.TargetType);
            }
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AutoMapAttribute : Attribute
    {
        public Type TargetType { get; }

        public AutoMapAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }


}
