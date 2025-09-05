using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Resumes.Updates.UpdateContactInfo;

internal class UpdateResumeContactInfoCommandValidator : AbstractValidator<UpdateResumeContactInfoCommand>
{
    public UpdateResumeContactInfoCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Email.MaxLength);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(PhoneNumber.MaxNumberLength);

        RuleFor(x => x.PhoneNumberRegionCode)
            .NotEmpty()
            .Length(PhoneNumber.RegionCodeLength);
    }
}
