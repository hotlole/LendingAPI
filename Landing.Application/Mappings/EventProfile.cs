using AutoMapper;
using Landing.Application.DTOs.Events;
using Landing.Core.Models.Events;
using Microsoft.AspNetCore.Http;

namespace Landing.Application.Mappings;

public class EventProfile : Profile
{
    public EventProfile()
    {
        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<RelativePathResolver<Event>>())
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImagePath));

        CreateMap<RegularEvent, EventDto>().IncludeBase<Event, EventDto>();
        CreateMap<CuratedEvent, EventDto>().IncludeBase<Event, EventDto>();
        CreateMap<OfflineEvent, EventDto>().IncludeBase<Event, EventDto>();

        CreateMap<CreateEventDto, Event>();
        CreateMap<CreateEventDto, RegularEvent>().IncludeBase<CreateEventDto, Event>();
        CreateMap<CreateEventDto, CuratedEvent>().IncludeBase<CreateEventDto, Event>();
        CreateMap<CreateEventDto, OfflineEvent>().IncludeBase<CreateEventDto, Event>();
    }
}
