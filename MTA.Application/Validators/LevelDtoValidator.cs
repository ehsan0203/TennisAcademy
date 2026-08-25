using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class LevelDtoValidator : AbstractValidator<CreateLevelDto>
{
    public LevelDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
    }
}
