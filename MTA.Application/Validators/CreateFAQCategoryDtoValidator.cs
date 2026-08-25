using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class CreateFAQCategoryDtoValidator : AbstractValidator<CreateFAQCategoryDto>
{
    public CreateFAQCategoryDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description is not null);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
