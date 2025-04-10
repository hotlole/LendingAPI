using Landing.Application.Mappings;
using AutoMapper;
using System;
using Swashbuckle.AspNetCore.Annotations;
using Landing.Core.Models.Events;

namespace Landing.Application.DTOs.Events
{
    /// <summary>
    /// DTO для мероприятия.
    /// </summary>
    [AutoMapper.AutoMap(typeof(Event))]
    public class EventDto : IMapWith<Event>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        [SwaggerSchema("Идентификатор мероприятия.")]
        public int Id { get; set; }

        /// <summary>
        /// Заголовок мероприятия.
        /// </summary>
        [SwaggerSchema("Заголовок мероприятия.")]
        public string Title { get; set; }

        /// <summary>
        /// Описание мероприятия.
        /// </summary>
        [SwaggerSchema("Описание мероприятия.")]
        public string Description { get; set; }

        /// <summary>
        /// Дата проведения мероприятия.
        /// </summary>
        [SwaggerSchema("Дата проведения мероприятия.")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Тип мероприятия.
        /// </summary>
        [SwaggerSchema("Тип мероприятия.")]
        public EventType Type { get; set; }

        /// <summary>
        /// Широта местоположения мероприятия.
        /// </summary>
        [SwaggerSchema("Широта местоположения мероприятия.")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Долгота местоположения мероприятия.
        /// </summary>
        [SwaggerSchema("Долгота местоположения мероприятия.")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Адрес проведения мероприятия.
        /// </summary>
        [SwaggerSchema("Адрес проведения мероприятия.")]
        public string? Address { get; set; }

        /// <summary>
        /// Пользовательский HTML-шаблон мероприятия.
        /// </summary>
        [SwaggerSchema("Пользовательский HTML-шаблон мероприятия.")]
        public string? CustomHtmlTemplate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Event, EventDto>();
        }
    }
}
