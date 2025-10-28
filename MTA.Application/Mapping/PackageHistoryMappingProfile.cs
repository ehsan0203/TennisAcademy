using AutoMapper;
using System;
using MTA.Domain.Entities;
using MTA.Application.DTOs;

namespace MTA.Application.Mapping;

/// <summary>
/// Mapping profile for PackageHistory entities
/// </summary>
public class PackageHistoryMappingProfile : BaseMappingProfile
{
    public PackageHistoryMappingProfile()
    {
        // PackageHistory to PackageHistoryDto mapping
        CreateMap<PackageHistory, PackageHistoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.ExpiredDate))
            .ForMember(dest => dest.RemainingCredits, opt => opt.MapFrom(src => src.RemainingCredits))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.PackageTitle, opt => opt.MapFrom(src => src.Package != null ? src.Package.Title : null))
            .ForMember(dest => dest.PackagePrice, opt => opt.MapFrom(src => src.PurchasePrice))
            .ForMember(dest => dest.TotalCredits, opt => opt.MapFrom(src => src.TotalCredits))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.Account != null && src.Account.UserProfile != null ? src.Account.UserProfile.FirstName : null))
            .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.Account != null && src.Account.UserProfile != null ? src.Account.UserProfile.LastName : null))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Account != null ? src.Account.Email : null))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiredDate < DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // PackageHistoryDto to PackageHistory mapping (for create/update operations)
        CreateMap<PackageHistoryDto, PackageHistory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.ExpiredDate))
            .ForMember(dest => dest.RemainingCredits, opt => opt.MapFrom(src => src.RemainingCredits))
            .ForMember(dest => dest.TotalCredits, opt => opt.MapFrom(src => src.TotalCredits))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PackagePrice))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Package, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore());

        CreateMap<CreatePackageHistoryDto, PackageHistory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.ExpiredDate))
            .ForMember(dest => dest.RemainingCredits, opt => opt.Ignore())
            .ForMember(dest => dest.TotalCredits, opt => opt.Ignore())
            .ForMember(dest => dest.PurchasePrice, opt => opt.Ignore())
            .ForMember(dest => dest.Package, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore());

    }
}
