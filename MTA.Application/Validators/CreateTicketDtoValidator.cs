using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class CreateTicketDtoValidator : AbstractValidator<CreateTicketDto>
{
    public CreateTicketDtoValidator()
    {
        RuleFor(x => x.Topic).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AccountId).GreaterThan(0);
    }
}
