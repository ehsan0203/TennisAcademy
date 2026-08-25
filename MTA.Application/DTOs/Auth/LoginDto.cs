using FluentValidation;

namespace MTA.Application.DTOs.Auth;

/// <summary>
/// Data Transfer Object for user login.
/// Validation lives in LoginDtoValidator (FluentValidation) — no DataAnnotations.
/// </summary>
public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}


public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}

