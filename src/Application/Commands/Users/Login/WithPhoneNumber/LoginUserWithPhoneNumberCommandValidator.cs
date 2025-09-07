using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Users.Login.WithPhoneNumber;

internal class LoginUserWithPhoneNumberCommandValidator : AbstractValidator<LoginUserWithPhoneNumberCommand>
{
    public LoginUserWithPhoneNumberCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(PhoneNumber.MaxNumberLength).WithMessage($"Phone number must not exceed {PhoneNumber.MaxNumberLength} characters.");

        RuleFor(x => x.PhoneNumberRegionCode)
            .NotEmpty().WithMessage("Region code is required.")
            .Length(PhoneNumber.RegionCodeLength).WithMessage($"Region code must be {PhoneNumber.RegionCodeLength} characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(User.MinPasswordLength).WithMessage($"Password must be at least {User.MinPasswordLength} characters long.")
            .MaximumLength(User.MaxPasswordLength).WithMessage($"Password must not exceed {User.MaxPasswordLength} characters.");
    }
}