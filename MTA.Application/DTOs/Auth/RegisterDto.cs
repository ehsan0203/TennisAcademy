using FluentValidation;

namespace MTA.Application.DTOs.Auth;

/// <summary>
/// Data Transfer Object for user registration.
/// Validation lives in RegisterDtoValidator (FluentValidation) — no DataAnnotations.
/// </summary>
public class RegisterDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Experience { get; set; }
    public int SkillLevelId { get; set; }
    public int RoleId { get; set; } = 1;
    public bool HealthCondition { get; set; }
    public string? HealthDescription { get; set; }
}

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Today).WithMessage("Date of birth cannot be in the future");

        RuleFor(x => x.DateOfBirth)
            .Must(d => d <= DateTime.Today.AddYears(-5))
            .WithMessage("User must be at least 5 years old");

        RuleFor(x => x.Experience)
            .InclusiveBetween(0, 50).WithMessage("Experience must be between 0 and 50 years");

        RuleFor(x => x.SkillLevelId)
            .GreaterThanOrEqualTo(0).WithMessage("Invalid skill level");

        When(x => x.HealthCondition, () =>
        {
            RuleFor(x => x.HealthDescription)
                .NotEmpty().WithMessage("Health description is required when HealthCondition is true")
                .MaximumLength(500).WithMessage("Health description cannot exceed 500 characters");
        });

        RuleFor(x => x.HealthDescription)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.HealthDescription))
            .WithMessage("Health description cannot exceed 500 characters");
    }
}


