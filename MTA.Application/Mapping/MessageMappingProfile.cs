using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;
using System.Linq;

namespace MTA.Application.Mapping;

public class MessageMappingProfile : BaseMappingProfile
{
    public MessageMappingProfile()
    {
        // --------------------------------
        // Message -> MessageDto mapping
        // --------------------------------
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
            .ForMember(dest => dest.TicketTopic, opt => opt.MapFrom(src => src.Ticket != null ? src.Ticket.Topic : null))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.SenderFirstName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.UserProfile.FirstName : null))
            .ForMember(dest => dest.SenderLastName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.UserProfile.LastName : null))

            // ???? MediaFile DTO??
            .ForMember(dest => dest.MediaFiles, opt => opt.MapFrom(src => src.MediaFiles.Select(mf => mf.MediaFile)))

            // ???? URL ??????? ???? ????? ?????
            .ForMember(dest => dest.MediaFileUrls, opt => opt.MapFrom(src => src.MediaFiles.Select(mf => mf.MediaFile.Url).ToList()))

            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // --------------------------------
        // CreateMessageDto -> Message mapping
        // --------------------------------
        CreateMap<CreateMessageDto, Message>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.MediaFiles, opt => opt.Ignore()) 
            .ForMember(dest => dest.Ticket, opt => opt.Ignore())
            .ForMember(dest => dest.Sender, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // --------------------------------
        // UpdateMessageDto -> Message mapping
        // --------------------------------
        CreateMap<UpdateMessageDto, Message>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.MediaFiles, opt => opt.Ignore()) 
            .ForMember(dest => dest.Ticket, opt => opt.Ignore())
            .ForMember(dest => dest.Sender, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
