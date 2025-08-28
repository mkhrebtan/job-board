using Domain.Abstraction;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.Entities;

public class Education : Entity<EducationId>
{
    private Education(string institutionName, string degree, string fieldOfStudy, DateRange dateRange)
        : base(new EducationId())
    {
        InstitutionName = institutionName;
        Degree = degree;
        FieldOfStudy = fieldOfStudy;
        DateRange = dateRange;
    }

    public string InstitutionName { get; private set; }

    public string Degree { get; private set; }

    public string FieldOfStudy { get; private set; }

    public DateRange DateRange { get; private set; }

    public static Result<Education> Create(string institutionName, string degree, string fieldOfStudy, DateRange dateRange)
    {
        if (string.IsNullOrWhiteSpace(institutionName))
        {
            return Result<Education>.Failure(new Error("Education.InvalidInstitutionName", "Institution name cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(degree))
        {
            return Result<Education>.Failure(new Error("Education.InvalidDegree", "Degree cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(fieldOfStudy))
        {
            return Result<Education>.Failure(new Error("Education.InvalidFieldOfStudy", "Field of study cannot be null or empty."));
        }

        if (dateRange == null)
        {
            return Result<Education>.Failure(new Error("Education.NullDateRange", "Date range cannot be null."));
        }

        var education = new Education(institutionName.Trim(), degree.Trim(), fieldOfStudy.Trim(), dateRange);
        return Result<Education>.Success(education);
    }
}