using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;

namespace MTA.Application.Mapping;

/// <summary>
/// Mapping profile for MediaFile entities
/// </summary>
public class MediaFileMappingProfile : BaseMappingProfile
{
    public MediaFileMappingProfile()
    {
        // MediaFile to MediaFileDto mapping
        CreateMap<MediaFile, MediaFileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type != null ? src.Type.Category : null))
            .ForMember(dest => dest.TypeValue, opt => opt.MapFrom(src => src.Type != null ? src.Type.Value : null))
            .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => src.LessonId))
            .ForMember(dest => dest.LessonTitle, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Title : null))
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => 0L)) // Placeholder - would need actual file size storage
            .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => Path.GetExtension(src.Url)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // MediaFileDto to MediaFile mapping (for create/update operations)
        CreateMap<MediaFileDto, MediaFile>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId))
            .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => src.LessonId))
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Lesson, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore());
    }
}
