using FluentValidation;

namespace MTA.Application.DTOs.Auth;

/// <summary>
/// Validates the forgot password request payload.
/// </summary>
public class ForgotPasswordRequestDtoValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");
    }
}
