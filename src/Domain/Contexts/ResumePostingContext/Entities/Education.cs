using Domain.Abstraction;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.Entities;

public class Education : Entity<EducationId>
{
    public const int MaxInstitutionNameLength = 200;
    public const int MaxDegreeLength = 100;
    public const int MaxFieldOfStudyLength = 100;

    private Education()
        : base(new EducationId())
    {
    }

    private Education(ResumeId resumeId, string institutionName, string degree, string fieldOfStudy, DateRange dateRange)
        : base(new EducationId())
    {
        ResumeId = resumeId;
        InstitutionName = institutionName;
        Degree = degree;
        FieldOfStudy = fieldOfStudy;
        DateRange = dateRange;
    }

    public ResumeId ResumeId { get; private set; }

    public string InstitutionName { get; private set; }

    public string Degree { get; private set; }

    public string FieldOfStudy { get; private set; }

    public DateRange DateRange { get; private set; }

    public static Result<Education> Create(ResumeId resumeId, string institutionName, string degree, string fieldOfStudy, DateRange dateRange)
    {
        if (resumeId == null || resumeId.Value == Guid.Empty)
        {
            return Result<Education>.Failure(new Error("Education.InvalidResumeId", "ResumeId cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(institutionName))
        {
            return Result<Education>.Failure(new Error("Education.InvalidInstitutionName", "Institution name cannot be null or empty."));
        }

        if (institutionName.Length > MaxInstitutionNameLength)
        {
            return Result<Education>.Failure(new Error("Education.InstitutionNameTooLong", $"Institution name cannot exceed {MaxInstitutionNameLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(degree))
        {
            return Result<Education>.Failure(new Error("Education.InvalidDegree", "Degree cannot be null or empty."));
        }

        if (degree.Length > MaxDegreeLength)
        {
            return Result<Education>.Failure(new Error("Education.DegreeTooLong", $"Degree cannot exceed {MaxDegreeLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(fieldOfStudy))
        {
            return Result<Education>.Failure(new Error("Education.InvalidFieldOfStudy", "Field of study cannot be null or empty."));
        }

        if (fieldOfStudy.Length > MaxFieldOfStudyLength)
        {
            return Result<Education>.Failure(new Error("Education.FieldOfStudyTooLong", $"Field of study cannot exceed {MaxFieldOfStudyLength} characters."));
        }

        if (dateRange == null)
        {
            return Result<Education>.Failure(new Error("Education.NullDateRange", "Date range cannot be null."));
        }

        var education = new Education(resumeId, institutionName.Trim(), degree.Trim(), fieldOfStudy.Trim(), dateRange);
        return Result<Education>.Success(education);
    }
}