using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Landing.Application.Mappings;
using Landing.Core.Models.Events;
using Swashbuckle.AspNetCore.Annotations;

namespace Landing.Application.DTOs.Events
{
    [SwaggerSchema("Модель, описывающая данные для создания нового мероприятия.")]
    public class CreateEventDto : IMapWith<Event>
    {
        [Required]
        [SwaggerSchema("Название мероприятия. Не может быть пустым.")]
        public string Title { get; set; }

        [Required]
        [SwaggerSchema("Описание мероприятия. Не может быть пустым.")]
        public string Description { get; set; }

        [Required]
        [SwaggerSchema("Дата проведения мероприятия. Не может быть пустой.")]
        public DateTime Date { get; set; }

        [Required]
        [SwaggerSchema("Тип мероприятия.")]
        [Range(1, 3, ErrorMessage = "Тип мероприятия должен быть Regular, Curated или Offline.")]
        public EventType Type { get; set; }


        [SwaggerSchema("Широта (только для очных мероприятий).")]
        public decimal? Latitude { get; set; }

        [SwaggerSchema("Долгота (только для очных мероприятий).")]
        public decimal? Longitude { get; set; }

        [SwaggerSchema("Адрес мероприятия (только для очных мероприятий).")]
        public string? Address { get; set; }

        [SwaggerSchema("HTML-шаблон мероприятия (только для очных мероприятий).")]
        public string? CustomHtmlTemplate { get; set; }
        public List<int>? CuratorIds { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateEventDto, Event>();
        }
    }
}
