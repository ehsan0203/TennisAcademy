using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class UpdateTicketDtoValidator : AbstractValidator<TicketDto>
{
    public UpdateTicketDtoValidator()
    {
        RuleFor(x => x.StatusId)
            .GreaterThan(0).WithMessage("StatusId must be greater than 0.");
    }
}
