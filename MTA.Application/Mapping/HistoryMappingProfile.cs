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
        // Mapping برای ایجاد رکورد جدید
        CreateMap<CreateUserCourseHistoryDto, UserCourseHistory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EnrolledAt, opt => opt.Ignore())
            .ForMember(dest => dest.StatusId, opt => opt.Ignore())
            .ForMember(dest => dest.PurchasePrice, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());

        // Mapping برای آپدیت / بازگرداندن اطلاعات
        CreateMap<UserCourseHistory, UpdateUserCourseHistoryDto>()
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
            .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PurchasePrice));

        CreateMap<UpdateUserCourseHistoryDto, UserCourseHistory>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.PurchasePrice, opt =>
            {
                opt.PreCondition(src => src.PurchasePrice.HasValue);
                opt.MapFrom(src => src.PurchasePrice!.Value);
            })
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());
    }
}
