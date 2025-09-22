using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;
using System.IO;

namespace MTA.Application.Mapping;

public class MediaFileMappingProfile : Profile
{
    public MediaFileMappingProfile()
    {
        // Entity -> DTO
        CreateMap<MediaFile, MediaFileDto>()
            .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => src.Type != null ? src.Type.Value : null))
            .ForMember(dest => dest.PlacementName, opt => opt.MapFrom(src => src.Placement != null ? src.Placement.Value : null))
            .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => src.FileExtension))
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize));

        // Upload DTO -> Entity
        CreateMap<MediaFileUploadDto, MediaFile>()
            .ForMember(dest => dest.TypeId, opt => opt.Ignore())       
            .ForMember(dest => dest.PlacementId, opt => opt.Ignore()) 
            .ForMember(dest => dest.Url, opt => opt.Ignore())          
            .ForMember(dest => dest.FileSize, opt => opt.Ignore())     
            .ForMember(dest => dest.FileExtension, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())   
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());  
    }
}

