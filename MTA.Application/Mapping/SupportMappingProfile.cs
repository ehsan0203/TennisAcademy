using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;

namespace MTA.Application.Mapping;

/// <summary>
/// Mapping profile for Ticket and Message entities
/// </summary>
public class SupportMappingProfile : BaseMappingProfile
{
    public SupportMappingProfile()
    {
        // Ticket mappings
        CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.Topic))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
            .ForMember(dest => dest.StatusValue, opt => opt.MapFrom(src => src.Status.Value)) // مقدار از Lookup
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.Account.UserProfile.FirstName))
            .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.Account.UserProfile.LastName))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.PackageTitle, opt => opt.MapFrom(src => src.Package.Title))
            .ForMember(dest => dest.MessageCount, opt => opt.MapFrom(src => src.Messages.Count));

        CreateMap<TicketDto, Ticket>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => src.Topic))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.Status, opt => opt.Ignore())   
            .ForMember(dest => dest.Account, opt => opt.Ignore())  
            .ForMember(dest => dest.Package, opt => opt.Ignore())  
            .ForMember(dest => dest.Messages, opt => opt.Ignore());


        // Message mappings
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))    
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
            .ForMember(dest => dest.TicketTopic, opt => opt.MapFrom(src => src.Ticket.Topic)) 
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.SenderFirstName, opt => opt.MapFrom(src => src.Sender.UserProfile.FirstName))
            .ForMember(dest => dest.SenderLastName, opt => opt.MapFrom(src => src.Sender.UserProfile.LastName))
            .ForMember(dest => dest.SenderImage, opt => opt.MapFrom(src => src.Sender.Image))
            .ForMember(dest => dest.MediaFileCount, opt => opt.MapFrom(src => src.MediaFiles.Count));

        CreateMap<MessageDto, Message>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.Ticket, opt => opt.Ignore()) 
            .ForMember(dest => dest.Sender, opt => opt.Ignore()) 
            .ForMember(dest => dest.MediaFiles, opt => opt.Ignore());


        // Question (FAQ) mappings
        // FAQCategory -> DTO
        CreateMap<FAQCategory, FAQCategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        // DTO -> FAQCategory
        CreateMap<FAQCategoryDto, FAQCategory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        // Question -> DTO
        CreateMap<QuestionFAQ, QuestionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
            .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.AnswerText))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.CategoryTitle, opt => opt.MapFrom(src => src.Category.Title))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        // DTO -> Question
        CreateMap<QuestionDto, QuestionFAQ>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
            .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.AnswerText))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive)); 

    }
}
