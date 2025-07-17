using FluentValidation;
using TennisAcademy.Application.DTOs.UserScore;

namespace TennisAcademy.Application.Validators;

public class AddCreditDtoValidator : AbstractValidator<AddCreditDto>
{
    public AddCreditDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
