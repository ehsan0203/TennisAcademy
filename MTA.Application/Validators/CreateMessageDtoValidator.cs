using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class CreateMessageDtoValidator : AbstractValidator<CreateMessageDto>
{
    public CreateMessageDtoValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.TicketId).GreaterThan(0);
        RuleFor(x => x.SenderId).GreaterThan(0);
    }
}
