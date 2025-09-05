using Domain.Contexts.ResumePostingContext.ValueObjects;
using FluentValidation;

namespace Application.Commands.Resumes.Updates.UpdatePersonalInfo;

internal class UpdateResumePersonalInfoCommandValidator : AbstractValidator<UpdateResumePersonalInfoCommand>
{
    public UpdateResumePersonalInfoCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(PersonalInfo.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(PersonalInfo.MaxLastNameLength);
    }
}
