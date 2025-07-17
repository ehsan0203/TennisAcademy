using FluentValidation;
using TennisAcademy.Application.DTOs.CoachMedia;

namespace TennisAcademy.Application.Validators;

public class CreateCoachMediaDtoValidator : AbstractValidator<CreateCoachMediaDto>
{
    public CreateCoachMediaDtoValidator()
    {
        RuleFor(x => x.CoachId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.FileUrl).NotEmpty();
    }
}
