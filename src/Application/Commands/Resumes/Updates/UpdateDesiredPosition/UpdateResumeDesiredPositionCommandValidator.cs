using Domain.Contexts.ResumePostingContext.ValueObjects;
using FluentValidation;

namespace Application.Commands.Resumes.Updates.UpdateDesiredPosition;

internal class UpdateResumeDesiredPositionCommandValidator : AbstractValidator<UpdateResumeDesiredPositionCommand>
{
    public UpdateResumeDesiredPositionCommandValidator()
    {
        RuleFor(x => x.DesiredPositionTitle)
            .NotEmpty()
            .MaximumLength(DesiredPosition.MaxLength);
    }
}
