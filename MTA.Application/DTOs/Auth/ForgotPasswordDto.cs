using FluentValidation;

namespace MTA.Application.DTOs.Auth;

public class ForgotPasswordDto
{
    public required string Email { get; set; }
}

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
