using FluentValidation;
using MTA.Application.DTOs.User;

namespace MTA.Application.Validators;

public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
{
    public CreateAccountDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("RoleId must be greater than 0.");

        RuleFor(x => x.StatusId)
            .GreaterThan(0).WithMessage("StatusId must be greater than 0.");
    }
}
