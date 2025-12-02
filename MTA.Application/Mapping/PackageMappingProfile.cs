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
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.TicketCount, opt => opt.MapFrom(src => src.TicketCount))
            .ForMember(dest => dest.MessageCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.DurationUnitId, opt => opt.MapFrom(src => src.DurationUnitId))
            .ForMember(dest => dest.DurationUnitValue, opt => opt.MapFrom(src => src.DurationUnit.Value))
            .ForMember(dest => dest.UsedTicketCount, opt => opt.Ignore()) 
            .ForMember(dest => dest.UsedMessageCount, opt => opt.Ignore()); 

        CreateMap<PackageDto, Package>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.TicketCount, opt => opt.MapFrom(src => src.TicketCount))
            .ForMember(dest => dest.MessageCount, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.DurationUnitId, opt => opt.MapFrom(src => src.DurationUnitId))
            .ForMember(dest => dest.DurationUnit, opt => opt.Ignore()) 
            .ForMember(dest => dest.Tickets, opt => opt.Ignore())
            .ForMember(dest => dest.PackageHistories, opt => opt.Ignore());


        // PackageHistory mappings
        CreateMap<PackageHistory, PackageHistoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.ExpiredDate))
            .ForMember(dest => dest.RemainingTickets, opt => opt.MapFrom(src => src.RemainingTickets))
            .ForMember(dest => dest.RemainingMessages, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.PackageTitle, opt => opt.MapFrom(src => src.Package.Title))
            .ForMember(dest => dest.PackagePrice, opt => opt.MapFrom(src => src.PurchasePrice))
            .ForMember(dest => dest.TotalTickets, opt => opt.MapFrom(src => src.Package.TicketCount))
            .ForMember(dest => dest.TotalMessages, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.Account.UserProfile.FirstName))
            .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.Account.UserProfile.LastName))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Account.Email))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiredDate < DateTime.Now));

        CreateMap<PackageHistoryDto, PackageHistory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.ExpiredDate))
            .ForMember(dest => dest.RemainingTickets, opt => opt.MapFrom(src => src.RemainingTickets))
            .ForMember(dest => dest.RemainingMessages, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.PackagePrice))
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.Package, opt => opt.Ignore());
    }
}
