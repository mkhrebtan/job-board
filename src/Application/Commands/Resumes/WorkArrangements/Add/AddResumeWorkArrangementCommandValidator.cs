using FluentValidation;

namespace Application.Commands.Resumes.WorkArrangements.Add;

internal class AddResumeWorkArrangementCommandValidator : AbstractValidator<AddResumeWorkArrangementCommand>
{
    public AddResumeWorkArrangementCommandValidator()
    {
        RuleForEach(x => x.WorkArrangements)
            .NotEmpty()
            .MaximumLength(25);
    }
}