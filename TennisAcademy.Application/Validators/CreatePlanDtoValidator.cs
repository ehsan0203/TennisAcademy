using FluentValidation;
using TennisAcademy.Application.DTOs.Plan;

namespace TennisAcademy.Application.Validators;

public class CreatePlanDtoValidator : AbstractValidator<CreatePlanDto>
{
    public CreatePlanDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
