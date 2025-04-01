using Landing.Core.Models;
using Landing.Application.Mappings;
using AutoMapper;
using System;
using Swashbuckle.AspNetCore.Annotations;

namespace Landing.Application.DTOs
{
    /// <summary>
    /// DTO для мероприятия.
    /// </summary>
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

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Event, EventDto>();
        }
    }
}
