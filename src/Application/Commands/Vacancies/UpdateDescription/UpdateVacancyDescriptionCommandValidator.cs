using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Vacancies.UpdateDescription;

internal class UpdateVacancyDescriptionCommandValidator : AbstractValidator<UpdateVacancyDescriptionCommand>
{
    public UpdateVacancyDescriptionCommandValidator()
    {
        RuleFor(x => x.descriptionMarkdown)
            .NotEmpty()
            .MaximumLength(RichTextContent.MaxLength);
    }
}
