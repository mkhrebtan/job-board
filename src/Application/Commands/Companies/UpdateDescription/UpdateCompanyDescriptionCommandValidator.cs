using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Companies.UpdateDescription;

internal class UpdateCompanyDescriptionCommandValidator : AbstractValidator<UpdateCompanyDescriptionCommand>
{
    public UpdateCompanyDescriptionCommandValidator()
    {
        RuleFor(x => x.DescriptionMarkdown)
            .NotEmpty()
            .MaximumLength(RichTextContent.MaxLength);
    }
}