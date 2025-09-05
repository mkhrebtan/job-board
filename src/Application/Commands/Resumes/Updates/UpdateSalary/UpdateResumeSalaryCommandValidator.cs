using FluentValidation;

namespace Application.Commands.Resumes.Updates.UpdateSalary;

internal class UpdateResumeSalaryCommandValidator : AbstractValidator<UpdateResumeSalaryCommand>
{
    public UpdateResumeSalaryCommandValidator()
    {
        RuleFor(x => x.SalaryAmount)
            .GreaterThan(0);

        RuleFor(x => x.SalaryCurrency)
            .NotEmpty()
            .Length(3);
    }
}
