using Landing.Core.Models;
using Landing.Application.Mappings;
using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace Landing.Application.DTOs
{
    public class UpdateEventDto : IMapWith<Event>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateEventDto, Event>();
        }
    }
}
