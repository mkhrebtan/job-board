using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Vacancies.Create;

internal class CreateVacancyCommandValidator : AbstractValidator<CreateVacancyCommand>
{
    public CreateVacancyCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(Vacancy.MaxTitleLength);

        RuleFor(x => x.DescriptionMarkdown)
            .NotEmpty()
            .MaximumLength(RichTextContent.MaxLength);

        RuleFor(x => x.MinSalary)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MaxSalary)
            .GreaterThanOrEqualTo(x => x.MinSalary)
            .When(x => x.MaxSalary.HasValue);

        RuleFor(x => x.SalaryCurrency)
            .NotEmpty()
            .Length(3)
            .When(x => x.SalaryCurrency is not null);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(Location.MaxCountryLength);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(Location.MaxCityLength);

        RuleFor(x => x.Region)
            .MaximumLength(Location.MaxRegionLength);

        RuleFor(x => x.District)
            .MaximumLength(Location.MaxDistrictLength);

        RuleFor(x => x.Address)
            .MaximumLength(Location.MaxAddressLength);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);

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
