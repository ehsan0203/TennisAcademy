using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
{
    public UpdateQuestionDtoValidator()
    {
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.AnswerText).NotEmpty().MaximumLength(5000);
    }
}
