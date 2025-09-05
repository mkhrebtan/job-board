using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Vacancies.UpdateRectuiterInfo;

internal class UpdateVacancyRecruiterInfoCommandValidator : AbstractValidator<UpdateVacancyRecruiterInfo>
{
    public UpdateVacancyRecruiterInfoCommandValidator()
    {
        RuleFor(x => x.RecruiterFirstName)
            .NotEmpty()
            .MaximumLength(RecruiterInfo.MaxFirstNameLength);

        RuleFor(x => x.RecruiterEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Email.MaxLength);

        RuleFor(x => x.RecruiterPhoneNumber)
            .NotEmpty()
            .MaximumLength(PhoneNumber.MaxNumberLength);

        RuleFor(x => x.RecruiterPhoneNumberRegionCode)
            .NotEmpty()
            .Length(PhoneNumber.RegionCodeLength);
    }
}
