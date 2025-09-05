using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Companies.UpdateWebsite;

internal class UpdateCompanyWebsiteCommandValidator : AbstractValidator<UpdateCompanyWebsiteCommand>
{
    public UpdateCompanyWebsiteCommandValidator()
    {
        RuleFor(x => x.WebsiteUrl)
            .NotEmpty()
            .MaximumLength(Url.MaxLength)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute));
    }
}