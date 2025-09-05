using FluentValidation;
using static Application.Commands.Resumes.Create.CreateResumeCommandValidator;

namespace Application.Commands.Resumes.Educations.Add;

internal class AddResumeEducationCommandValidator : AbstractValidator<AddResumeEducationCommand>
{
    public AddResumeEducationCommandValidator()
    {
        RuleFor(x => x.Education)
            .NotNull()
            .SetValidator(new EducationDtoValidator());
    }
}
