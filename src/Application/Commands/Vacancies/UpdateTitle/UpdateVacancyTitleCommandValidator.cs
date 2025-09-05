using Domain.Contexts.JobPostingContext.Aggregates;
using FluentValidation;

namespace Application.Commands.Vacancies.UpdateTitle;

internal class UpdateVacancyTitleCommandValidator : AbstractValidator<UpdateVacancyTitleCommand>
{
    public UpdateVacancyTitleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(Vacancy.MaxTitleLength);
    }
}
