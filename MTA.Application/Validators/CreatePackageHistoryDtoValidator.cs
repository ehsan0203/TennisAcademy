using FluentValidation;
using MTA.Application.DTOs;

namespace MTA.Application.Validators;

public class CreatePackageHistoryDtoValidator : AbstractValidator<CreatePackageHistoryDto>
{
    public CreatePackageHistoryDtoValidator()
    {
        RuleFor(x => x.PackageId).GreaterThan(0);
        RuleFor(x => x.AccountId).GreaterThan(0);
    }
}
