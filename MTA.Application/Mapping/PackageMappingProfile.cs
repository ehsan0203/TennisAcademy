using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;

namespace MTA.Application.Mapping;

/// <summary>
/// Mapping profile for Package and MediaFile entities
/// </summary>
public class PackageMappingProfile : BaseMappingProfile
{
    public PackageMappingProfile()
    {
        // Package mappings
        CreateMap<Package, PackageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.CreditCount, opt => opt.MapFrom(src => src.CreditCount))
            .ForMember(dest => dest.UsedCreditCount, opt => opt.MapFrom(src => src.Tickets != null ? src.Tickets.Count : 0));

        CreateMap<PackageDto, Package>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.CreditCount, opt => opt.MapFrom(src => src.CreditCount))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.Tickets, opt => opt.Ignore())
            .ForMember(dest => dest.PackageHistories, opt => opt.Ignore());


        // PackageHistory mappings
        CreateMap<PackageHistory, PackageHistoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.ExpiredDate))
            .ForMember(dest => dest.RemainingCredits, opt => opt.MapFrom(src => src.RemainingCredits))
            .ForMember(dest => dest.TotalCredits, opt => opt.MapFrom(src => src.TotalCredits))
            .ForMember(dest => dest.PackageTitle, opt => opt.MapFrom(src => src.Package.Title))
            .ForMember(dest => dest.PackagePrice, opt => opt.MapFrom(src => src.PurchasePrice))
            .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.Account.UserProfile.FirstName))
            .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.Account.UserProfile.LastName))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Account.Email))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiredDate < DateTime.Now));

        CreateMap<PackageHistoryDto, PackageHistory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.ExpiredDate))
            .ForMember(dest => dest.RemainingCredits, opt => opt.MapFrom(src => src.RemainingCredits))
            .ForMember(dest => dest.TotalCredits, opt => opt.MapFrom(src => src.TotalCredits))
            .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PackagePrice))
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.Package, opt => opt.Ignore());
    }
}
