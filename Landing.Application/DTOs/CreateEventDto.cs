using Landing.Core.Models;
using Landing.Application.Mappings;
using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace Landing.Application.DTOs
{
    public class CreateEventDto : IMapWith<Event>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateEventDto, Event>();
        }
    }
}
