using AutoMapper;
using Landing.Application.DTOs.Events;
using Landing.Core.Models.Events;

namespace Landing.Application.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            // RegularEvent
            CreateMap<CreateEventDto, RegularEvent>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => EventType.Regular));

            // CuratedEvent
            CreateMap<CreateEventDto, CuratedEvent>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => EventType.Curated));

            // OfflineEvent
            CreateMap<CreateEventDto, OfflineEvent>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => EventType.Offline))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.CustomHtmlTemplate, opt => opt.MapFrom(src => src.CustomHtmlTemplate));
            CreateMap<Event, EventDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<RelativePathResolver<Event>>());

            CreateMap<CreateEventDto, Event>()
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore());

            CreateMap<UpdateEventDto, Event>()
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore());
            // Для ответа
            CreateMap<Event, EventDto>()
                .Include<RegularEvent, EventDto>()
                .Include<CuratedEvent, EventDto>()
                .Include<OfflineEvent, EventDto>();

            CreateMap<RegularEvent, EventDto>();
            CreateMap<CuratedEvent, EventDto>();
            CreateMap<OfflineEvent, EventDto>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.CustomHtmlTemplate, opt => opt.MapFrom(src => src.CustomHtmlTemplate));
        }
    }
}
