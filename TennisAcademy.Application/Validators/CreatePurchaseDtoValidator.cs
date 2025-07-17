using FluentValidation;
using TennisAcademy.Application.DTOs.Purchase;

namespace TennisAcademy.Application.Validators;

public class CreatePurchaseDtoValidator : AbstractValidator<CreatePurchaseDto>
{
    public CreatePurchaseDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.CourseId.HasValue || x.PlanId.HasValue)
            .WithMessage("Either CourseId or PlanId is required.");
    }
}
