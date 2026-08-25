using FluentValidation;
using MTA.Application.DTOs.User;

namespace MTA.Application.Validators;

public class UpdateCurrentUserDtoValidator : AbstractValidator<UpdateCurrentUserDto>
{
    public UpdateCurrentUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required.")
            .When(x => x.Email is not null);

        RuleFor(x => x.SkillLevelId)
            .GreaterThanOrEqualTo(0).WithMessage("SkillLevelId must be 0 or greater.")
            .When(x => x.SkillLevelId.HasValue);
    }
}
