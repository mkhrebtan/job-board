using FluentValidation;

namespace Application.Commands.Vacancies.UpdateSalary;

internal class UpdateVacancySalaryCommandValidator : AbstractValidator<UpdateVacancySalaryCommand>
{
    public UpdateVacancySalaryCommandValidator()
    {
        RuleFor(x => x.MinSalary)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MaxSalary)
            .GreaterThanOrEqualTo(x => x.MinSalary)
                .When(x => x.MaxSalary.HasValue);

        RuleFor(x => x.SalaryCurrency)
            .NotEmpty()
            .Length(3);
    }
}