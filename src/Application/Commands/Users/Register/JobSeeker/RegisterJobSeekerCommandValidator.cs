using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Users.Register.JobSeeker;

internal class RegisterJobSeekerCommandValidator : AbstractValidator<RegisterJobSeekerCommand>
{
    public RegisterJobSeekerCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(User.MaxFirstNameLength).WithMessage($"First name must not exceed {User.MaxFirstNameLength} characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(User.MaxLastNameLength).WithMessage($"Last name must not exceed {User.MaxLastNameLength} characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.")
            .MaximumLength(Email.MaxLength).WithMessage($"Email must not exceed {Email.MaxLength} characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(PhoneNumber.MaxNumberLength).WithMessage($"Phone number must not exceed {PhoneNumber.MaxNumberLength} characters.");

        RuleFor(x => x.PhoneNumberRegionCode)
            .NotEmpty().WithMessage("Phone number region code is required.")
            .Length(PhoneNumber.RegionCodeLength).WithMessage($"Phone number region code must be {PhoneNumber.RegionCodeLength} characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(User.MinPasswordLength).WithMessage($"Password must be at least {User.MinPasswordLength} characters long.")
            .MaximumLength(User.MaxPasswordLength).WithMessage($"Password must not exceed {User.MaxPasswordLength} characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}