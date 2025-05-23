﻿using AutoMapper;
using System.Reflection;
using System.Linq;
using Landing.Core.Models.Events;
using Landing.Application.DTOs.Events;
namespace Landing.Application.Mappings
{
    public class AssemblyMappingProfile : Profile
    {
        public AssemblyMappingProfile(Assembly assembly) =>
            ApplyMappingsFromAssembly(assembly);
        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IMapWith<>)))
                 .ToList();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping");
                methodInfo?.Invoke(instance, new object[] { this });
            }

        }
        public AssemblyMappingProfile()
        {
            CreateMap<CreateEventDto, Event>(); // Добавляем маппинг
        }

    }
}
