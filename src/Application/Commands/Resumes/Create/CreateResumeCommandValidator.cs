using Application.Commands.Resumes.Dtos;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.ResumePostingContext.Entities;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace Application.Commands.Resumes.Create;

internal class CreateResumeCommandValidator : AbstractValidator<CreateResumeCommand>
{
    public CreateResumeCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(PersonalInfo.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(PersonalInfo.MaxLastNameLength);

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

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);

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

        RuleFor(x => x.DesiredPositionTitle)
            .NotEmpty()
            .MaximumLength(DesiredPosition.MaxLength);

        RuleFor(x => x.SalaryAmount)
            .GreaterThan(0);

        RuleFor(x => x.SalaryCurrency)
            .NotEmpty()
            .Length(3);

        RuleFor(x => x.SkillsDescripitonMarkdown)
            .NotEmpty()
            .MaximumLength(RichTextContent.MaxLength);

        RuleForEach(x => x.EmploymentTypes)
            .NotEmpty()
            .MaximumLength(25);

        RuleForEach(x => x.WorkArrangements)
            .NotEmpty()
            .MaximumLength(25);

        RuleForEach(x => x.WorkExpetiences)
            .SetValidator(new WorkExpetienceDtoValidator());
        RuleForEach(x => x.Educations)
            .SetValidator(new EducationDtoValidator());
        RuleForEach(x => x.Languages)
            .SetValidator(new LanguageSkillDtoValidator());
    }

    internal sealed class WorkExpetienceDtoValidator : AbstractValidator<WorkExperienceDto>
    {
        public WorkExpetienceDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .MaximumLength(WorkExperience.MaxCompanyNameLength);

            RuleFor(x => x.Position)
                .NotEmpty()
                .MaximumLength(WorkExperience.MaxPositionLength);

            RuleFor(x => x.DescriptionMarkdown)
                .NotEmpty()
                .MaximumLength(RichTextContent.MaxLength);

            RuleFor(x => x.StartDate)
                .NotEmpty();

            RuleFor(x => x.EndDate)
                .Must((dto, endDate) => endDate == null ||
                    (endDate > dto.StartDate && (endDate.Value.Year != dto.StartDate.Year || endDate.Value.Month != dto.StartDate.Month)))
                .WithMessage("End date must be greater than start date and cannot be in the same year and month as start date")
                .When(x => x.EndDate.HasValue);
        }
    }

    internal sealed class EducationDtoValidator : AbstractValidator<EducationDto>
    {
        public EducationDtoValidator()
        {
            RuleFor(x => x.InstitutionName)
                .NotEmpty()
                .MaximumLength(Education.MaxInstitutionNameLength);

            RuleFor(x => x.Degree)
                .NotEmpty()
                .MaximumLength(Education.MaxDegreeLength);

            RuleFor(x => x.FieldOfStudy)
                .NotEmpty()
                .MaximumLength(Education.MaxFieldOfStudyLength);

            RuleFor(x => x.StartDate)
                .NotEmpty();

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue);
        }
    }

    internal sealed class LanguageSkillDtoValidator : AbstractValidator<LanguageSkillDto>
    {
        public LanguageSkillDtoValidator()
        {
            RuleFor(x => x.Language)
                .NotEmpty()
                .MaximumLength(LanguageSkill.MaxLanguageLength);

            RuleFor(x => x.ProficiencyLevel)
                .NotEmpty()
                .MaximumLength(2);
        }
    }
}
