using Domain.Abstraction;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.ResumePostingContext.Entities;

public class WorkExperience : Entity<WorkExperienceId>
{
    private WorkExperience(string companyName, string position, DateRange dateRange, RichTextContent description)
        : base(new WorkExperienceId())
    {
        CompanyName = companyName;
        Position = position;
        DateRange = dateRange;
        Description = description;
    }

    public string CompanyName { get; private set; }

    public string Position { get; private set; }

    public DateRange DateRange { get; private set; }

    public RichTextContent Description { get; private set; }

    public static Result<WorkExperience> Create(string companyName, string position, DateRange dateRange, RichTextContent description)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.InvalidCompanyName", "Company name cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(position))
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.InvalidPosition", "Position cannot be null or empty."));
        }

        if (dateRange == null)
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.NullDateRange", "Date range cannot be null."));
        }

        if (description == null)
        {
            return Result<WorkExperience>.Failure(new Error("WorkExperience.NullDescription", "Description cannot be null."));
        }

        return Result<WorkExperience>.Success(new WorkExperience(companyName.Trim(), position.Trim(), dateRange, description));
    }
}