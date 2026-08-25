using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class CreatePackageDtoValidator : AbstractValidator<CreatePackageDto>
{
    public CreatePackageDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TicketCount).GreaterThan(0);
        RuleFor(x => x.Duration).GreaterThan(0);
        RuleFor(x => x.DurationUnitId).GreaterThan(0);
    }
}
