using Landing.Core.Models;
using Landing.Application.Mappings;
using AutoMapper;
using System;

namespace Landing.Application.DTOs
{
    public class EventDto : IMapWith<Event>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Event, EventDto>();
        }
    }
}
