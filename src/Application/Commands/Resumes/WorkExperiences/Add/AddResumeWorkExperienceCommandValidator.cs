using Application.Commands.Resumes.Create;
using FluentValidation;

namespace Application.Commands.Resumes.WorkExperiences.Add;

internal class AddResumeWorkExperienceCommandValidator : AbstractValidator<AddResumeWorkExperienceCommand>
{
    public AddResumeWorkExperienceCommandValidator()
    {
        RuleFor(x => x.WorkExperience).NotNull().SetValidator(new CreateResumeCommandValidator.WorkExpetienceDtoValidator());
    }
}