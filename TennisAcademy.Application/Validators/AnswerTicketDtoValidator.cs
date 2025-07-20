using FluentValidation;
using TennisAcademy.Application.DTOs.Ticket;

namespace TennisAcademy.Application.Validators;

public class AnswerTicketDtoValidator : AbstractValidator<AnswerTicketDto>
{
    public AnswerTicketDtoValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        // At least one response should be provided
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.CoachReply) ||
                       !string.IsNullOrWhiteSpace(x.CoachReplyVoiceUrl) ||
                       !string.IsNullOrWhiteSpace(x.CoachReplyVideoUrl) ||
                       x.CoachVoiceMediaId.HasValue ||
                       x.CoachVideoMediaId.HasValue)
            .WithMessage("Reply content is required.");
    }
}
