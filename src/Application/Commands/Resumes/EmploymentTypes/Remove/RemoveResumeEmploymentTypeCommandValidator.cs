using FluentValidation;

namespace Application.Commands.Resumes.EmploymentTypes.Remove;

internal class RemoveResumeEmploymentTypeCommandValidator : AbstractValidator<RemoveResumeEmploymentTypeCommand>
{
    public RemoveResumeEmploymentTypeCommandValidator()
    {
        RuleForEach(x => x.EmploymentTypes)
            .NotEmpty()
            .MaximumLength(25);
    }
}
