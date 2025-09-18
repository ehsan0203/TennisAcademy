using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MTA.Application.DTOs.Auth;

/// <summary>
/// Data Transfer Object for user registration
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    /// <summary>
    /// User's password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public required string Password { get; set; }

    /// <summary>
    /// Password confirmation
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }

    /// <summary>
    /// User's first name
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public required string LastName { get; set; }

    /// <summary>
    /// User's date of birth
    /// </summary>
    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// User's tennis experience in years
    /// </summary>
    [Required(ErrorMessage = "Experience is required")]
    [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
    public int Experience { get; set; }

    /// <summary>
    /// User's skill level ID
    /// </summary>
    [Required(ErrorMessage = "Skill level is required")]
    public int SkillLevelId { get; set; }
    
    /// <summary>
    /// User's role ID (default to student role)
    /// </summary>
    public int RoleId { get; set; } = 1;
    public bool HealthCondition { get; set; }
    [StringLength(500, ErrorMessage = "Health Description cannot exceed 100 characters")]
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


