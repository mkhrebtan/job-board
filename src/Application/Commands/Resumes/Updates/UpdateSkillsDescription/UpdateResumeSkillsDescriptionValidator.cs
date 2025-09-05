using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Resumes.Updates.UpdateSkillsDescription;

internal class UpdateResumeSkillsDescriptionValidator : AbstractValidator<UpdateResumeSkillsDescription>
{
    public UpdateResumeSkillsDescriptionValidator()
    {
        RuleFor(x => x.SkillsDescripitonMarkdown)
           .NotEmpty()
           .MaximumLength(RichTextContent.MaxLength);
    }
}
