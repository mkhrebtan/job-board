using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Companies.UpdateLogo;

internal class UpdateCompanyLogoCommandValidator : AbstractValidator<UpdateCompanyLogoCommand>
{
    public UpdateCompanyLogoCommandValidator()
    {
        RuleFor(x => x.LogoUrl)
            .NotEmpty()
            .MaximumLength(Url.MaxLength)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute));
    }
}