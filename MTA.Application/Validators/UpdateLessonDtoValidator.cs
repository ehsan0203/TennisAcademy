using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class UpdateLessonDtoValidator : AbstractValidator<UpdateLessonDto>
{
    public UpdateLessonDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
        RuleFor(x => x.CourseId).GreaterThan(0);
    }
}
