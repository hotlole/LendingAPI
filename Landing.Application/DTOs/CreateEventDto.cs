using Landing.Core.Models;
using Landing.Application.Mappings;
using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Landing.Application.DTOs
{
    /// <summary>
    /// Модель для создания нового мероприятия.
    /// </summary>
    [SwaggerSchema("Модель, описывающая данные для создания нового мероприятия.")]
    public class CreateEventDto : IMapWith<Event>
    {
        /// <summary>
        /// Название мероприятия.
        /// </summary>
        [Required]
        [SwaggerSchema("Название мероприятия. Не может быть пустым.")]
        public string Title { get; set; }

        /// <summary>
        /// Описание мероприятия.
        /// </summary>
        [Required]
        [SwaggerSchema("Описание мероприятия. Не может быть пустым.")]
        public string Description { get; set; }

        /// <summary>
        /// Дата проведения мероприятия.
        /// </summary>
        [Required]
        [SwaggerSchema("Дата проведения мероприятия. Не может быть пустой.")]
        public DateTime Date { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateEventDto, Event>();
        }
    }
}
