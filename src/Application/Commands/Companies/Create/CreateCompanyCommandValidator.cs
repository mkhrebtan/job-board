using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Companies.Create;

internal class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Company.MaxNameLength);

        RuleFor(x => x.DescriptionMarkdown)
            .NotEmpty()
            .MaximumLength(RichTextContent.MaxLength);

        RuleFor(x => x.WebsiteUrl)
            .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute));

        RuleFor(x => x.LogoUrl)
            .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute));

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .When(x => x.Size.HasValue);
    }
}