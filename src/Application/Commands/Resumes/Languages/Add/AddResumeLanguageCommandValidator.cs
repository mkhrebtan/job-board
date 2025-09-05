using FluentValidation;
using static Application.Commands.Resumes.Create.CreateResumeCommandValidator;

namespace Application.Commands.Resumes.Languages.Add;

internal class AddResumeLanguageCommandValidator : AbstractValidator<AddResumeLanguageCommand>
{
    public AddResumeLanguageCommandValidator()
    {
        RuleFor(x => x.Language)
            .NotNull()
            .SetValidator(new LanguageSkillDtoValidator());
    }
}