using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;

namespace MTA.Application.Mapping;

/// <summary>
/// Mapping profile for Message entities
/// </summary>
public class MessageMappingProfile : BaseMappingProfile
{
	public MessageMappingProfile()
	{
		// Message to MessageDto mapping
		CreateMap<Message, MessageDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
			.ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
			.ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
			.ForMember(dest => dest.TicketTopic, opt => opt.MapFrom(src => src.Ticket != null ? src.Ticket.Topic : null))
			.ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
			.ForMember(dest => dest.SenderFirstName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.UserProfile.FirstName : null))
			.ForMember(dest => dest.SenderLastName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.UserProfile.LastName : null))
			.ForMember(dest => dest.SenderImage, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.MediaFile.Url : null))
			.ForMember(dest => dest.MediaFileId, opt => opt.MapFrom(src => src.MediaFileId))
			.ForMember(dest => dest.MediaFileUrl, opt => opt.MapFrom(src => src.MediaFile != null ? src.MediaFile.Url : null))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

		// MessageDto to Message mapping (for create/update operations)
		CreateMap<MessageDto, Message>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
			.ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
			.ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
			.ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
			.ForMember(dest => dest.MediaFileId, opt => opt.MapFrom(src => src.MediaFileId))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
			.ForMember(dest => dest.Ticket, opt => opt.Ignore())
			.ForMember(dest => dest.Sender, opt => opt.Ignore())
			.ForMember(dest => dest.MediaFile, opt => opt.Ignore());
	}
}
