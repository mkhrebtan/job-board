using FluentValidation;

namespace Application.Commands.Resumes.WorkArrangements.Remove;

internal class RemoveResumeWorkArrangementCommandValidator : AbstractValidator<RemoveResumeWorkArrangementCommand>
{
    public RemoveResumeWorkArrangementCommandValidator()
    {
        RuleForEach(x => x.WorkArrangements)
            .NotEmpty()
            .MaximumLength(25);
    }
}
