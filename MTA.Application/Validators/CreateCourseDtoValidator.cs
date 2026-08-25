using FluentValidation;
using MTA.Application.DTOs.Course;

namespace MTA.Application.Validators;

public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LevelId).GreaterThan(0);
        RuleFor(x => x.StatusId).GreaterThanOrEqualTo(0);
    }
}
