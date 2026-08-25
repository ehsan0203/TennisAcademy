using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionDtoValidator()
    {
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.AnswerText).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}
