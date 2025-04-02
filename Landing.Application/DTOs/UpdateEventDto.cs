using Landing.Application.Mappings;
using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Landing.Core.Models.Events;

namespace Landing.Application.DTOs
{
    /// <summary>
    /// Модель для обновления мероприятия.
    /// </summary>
    [SwaggerSchema("Модель, описывающая данные для обновления мероприятия.")]
    public class UpdateEventDto : IMapWith<Event>
    {
        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        [Required]
        [SwaggerSchema("Уникальный идентификатор мероприятия.")]
        public int Id { get; set; }

        /// <summary>
        /// Заголовок мероприятия.
        /// </summary>
        [Required]
        [SwaggerSchema("Заголовок мероприятия. Обязательное поле.")]
        public string Title { get; set; }

        /// <summary>
        /// Описание мероприятия.
        /// </summary>
        [Required]
        [SwaggerSchema("Описание мероприятия. Обязательное поле.")]
        public string Description { get; set; }

        /// <summary>
        /// Дата мероприятия.
        /// </summary>
        [Required]
        [SwaggerSchema("Дата проведения мероприятия.")]
        public DateTime Date { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateEventDto, Event>();
        }
    }
}
