using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.VacancyApplications.ApplyWithFile;

internal class ApplyToVacancyWithFileCommandValidator : AbstractValidator<ApplyToVacancyWithFileCommand>
{
    public ApplyToVacancyWithFileCommandValidator()
    {
        RuleFor(x => x.CoverLetterContent)
            .MaximumLength(CoverLetter.MaxLength);

        RuleFor(x => x.FileUrl)
            .NotEmpty()
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .MaximumLength(Url.MaxLength);
    }
}
