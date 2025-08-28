using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;

namespace MTA.Application.Mapping;

/// <summary>
/// Mapping profile for UserCourseHistory entity
/// </summary>
public class HistoryMappingProfile : BaseMappingProfile
{
    public HistoryMappingProfile()
    {
        // UserCourseHistory mappings
        CreateMap<UserCourseHistory, UserCourseHistoryDto>()
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
            .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.Course.Description))
            .ForMember(dest => dest.CourseImageIcon, opt => opt.MapFrom(src => src.Course.ImageIcon))
            .ForMember(dest => dest.CoursePrice, opt => opt.MapFrom(src => src.Course.Price))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.Account.UserProfile.FirstName))
            .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.Account.UserProfile.LastName))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Account.Email));

        CreateMap<UserCourseHistoryDto, UserCourseHistory>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.Account, opt => opt.Ignore()) 
            .ForMember(dest => dest.Course, opt => opt.Ignore());



    }
}
