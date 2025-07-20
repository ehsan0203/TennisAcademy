using FluentValidation;
using TennisAcademy.Application.DTOs.Ticket;

namespace TennisAcademy.Application.Validators;

public class CreateTicketDtoValidator : AbstractValidator<CreateTicketDto>
{
    public CreateTicketDtoValidator()
    {
        RuleFor(x => x.Subject).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
