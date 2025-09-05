using FluentValidation;

namespace Application.Commands.Resumes.EmploymentTypes.Add;

internal class AddResumeEmploymentTypeCommandValidator : AbstractValidator<AddResumeEmploymentTypeCommand>
{
    public AddResumeEmploymentTypeCommandValidator()
    {
        RuleForEach(x => x.EmploymentTypes)
            .NotEmpty()
            .MaximumLength(25);
    }
}
