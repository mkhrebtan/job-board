using Domain.Contexts.JobPostingContext.ValueObjects;
using FluentValidation;

namespace Application.Commands.Vacancies.UpdateLocation;

internal class UpdateVacancyLocationCommandValidator : AbstractValidator<UpdateVacancyLocationCommand>
{
    public UpdateVacancyLocationCommandValidator()
    {
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
    }
}
