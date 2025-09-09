using FluentValidation;

namespace Application.Commands.Users.RefreshTokens;

internal class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Refresh token must be provided.");
    }
}
