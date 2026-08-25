using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class PaymentInitRequestDtoValidator : AbstractValidator<PaymentInitRequestDto>
{
    public PaymentInitRequestDtoValidator()
    {
        RuleFor(x => x.AccountId).GreaterThan(0);
        RuleFor(x => x.SuccessUrl).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("SuccessUrl must be a valid absolute URL.");
    }
}
