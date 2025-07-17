using FluentValidation;
using TennisAcademy.Application.DTOs.Course;

namespace TennisAcademy.Application.Validators;

public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CoverImageUrl).NotEmpty();
        RuleFor(x => x.VideoIntroUrl).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
