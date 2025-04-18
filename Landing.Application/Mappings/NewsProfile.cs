﻿using AutoMapper;
using Landing.Application.DTOs.News;
using Landing.Core.Models.News;

public class NewsProfile : Profile
{
    public NewsProfile()
    {
        CreateMap<News, NewsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedAt));

        CreateMap<CreateNewsDto, News>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()) // Это будет задаваться после загрузки
            .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Content, opt => opt.Ignore())
            .ForMember(dest => dest.VideoUrl, opt => opt.Ignore())
            .ForMember(dest => dest.VideoPreviewUrl, opt => opt.Ignore())
            .ForMember(dest => dest.ExternalLink, opt => opt.Ignore())
            .ForMember(dest => dest.LinkTitle, opt => opt.Ignore())
            .ForMember(dest => dest.AdditionalImages, opt => opt.Ignore());

        CreateMap<UpdateNewsDto, News>()
            .ForMember(dest => dest.Title, opt => opt.Condition(src => src.Title != null))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); 
            
    }
}
