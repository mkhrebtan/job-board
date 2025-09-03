using Domain.Abstraction;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.ResumePostingContext.Entities;

public class WorkExperience : Entity<WorkExperienceId>
{
    public const int MaxCompanyNameLength = Company.MaxNameLength;
    public const int MaxPositionLength = 100;

    private WorkExperience()
        : base(new WorkExperienceId())
    {
    }

    private WorkExperience(ResumeId resumeId, string companyName, string position, DateRange dateRange, RichTextContent description)
        : base(new WorkExperienceId())
    {
        ResumeId = resumeId;
        CompanyName = companyName;
        Position = position;
        DateRange = dateRange;
        Description = description;
    }

    public ResumeId ResumeId { get; private set; }

    public string CompanyName { get; private set; }

    public string Position { get; private set; }

    public DateRange DateRange { get; private set; }

    public RichTextContent Description { get; private set; }

    public static Result<WorkExperience> Create(ResumeId resumeId, string companyName, string position, DateRange dateRange, RichTextContent description)
    {
        if (resumeId == null || resumeId.Value == Guid.Empty)
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.InvalidResumeId", "ResumeId cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(companyName))
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.InvalidCompanyName", "Company name cannot be null or empty."));
        }

        if (companyName.Length > MaxCompanyNameLength)
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.CompanyNameTooLong", $"Company name cannot exceed {MaxCompanyNameLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(position))
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.InvalidPosition", "Position cannot be null or empty."));
        }

        if (position.Length > MaxPositionLength)
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.PositionTooLong", $"Position name cannot exceed {MaxPositionLength} characters."));
        }

        if (dateRange == null)
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.NullDateRange", "Date range cannot be null."));
        }

        if (description == null)
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.NullDescription", "Description cannot be null."));
        }

        return Result<WorkExperience>.Success(new WorkExperience(resumeId, companyName.Trim(), position.Trim(), dateRange, description));
    }
}