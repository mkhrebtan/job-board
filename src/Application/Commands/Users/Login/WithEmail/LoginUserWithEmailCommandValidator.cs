using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Users.Login.WithEmail;

internal class LoginUserWithEmailCommandValidator : AbstractValidator<LoginUserWithEmailCommand>
{
    public LoginUserWithEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(Email.MaxLength).WithMessage($"Email must not exceed {Email.MaxLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(User.MinPasswordLength).WithMessage($"Password must be at least {User.MinPasswordLength} characters long.")
            .MaximumLength(User.MaxPasswordLength).WithMessage($"Password must not exceed {User.MaxPasswordLength} characters.");
    }
}
