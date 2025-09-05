using Domain.Abstraction;
using Domain.Contexts.JobPostingContext.Enums;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.JobPostingContext.Aggregates;

public class Vacancy : AggregateRoot<VacancyId>
{
    public const int MaxTitleLength = 100;

    private Vacancy()
        : base(new VacancyId())
    {
    }

    private Vacancy(
        VacancyTitle title,
        RichTextContent descripiton,
        VacancyStatus status,
        Salary salary,
        CompanyId companyId,
        Location location,
        RecruiterInfo recruiterInfo)
            : base(new VacancyId())
    {
        Title = title;
        Description = descripiton;
        Status = status;
        Salary = salary;
        CompanyId = companyId;
        Location = location;
        RecruiterInfo = recruiterInfo;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public VacancyTitle Title { get; private set; }

    public RichTextContent Description { get; private set; }

    public VacancyStatus Status { get; private set; }

    public CategoryId? CategoryId { get; private set; } = null!;

    public Salary Salary { get; private set; }

    public CompanyId CompanyId { get; private set; }

    public Location Location { get; private set; }

    public RecruiterInfo RecruiterInfo { get; private set; }

    public DateTime CreatedAt { get; private init; }

    public DateTime? RegisteredAt { get; private set; }

    public DateTime? PublishedAt { get; private set; }

    public DateTime LastUpdatedAt { get; private set; }

    internal static Result<Vacancy> CreateDraft(
        VacancyTitle title,
        RichTextContent descripiton,
        Salary salary,
        CompanyId companyId,
        Location location,
        RecruiterInfo recruiterInfo)
    {
        return Create(title, descripiton, VacancyStatus.Draft, salary, companyId, location, recruiterInfo);
    }

    internal static Result<Vacancy> CreateAndRegister(
        VacancyTitle title,
        RichTextContent descripiton,
        Salary salary,
        CompanyId companyId,
        Location location,
        RecruiterInfo recruiterInfo)
    {
        var result = Create(title, descripiton, VacancyStatus.Registered, salary, companyId, location, recruiterInfo);
        if (result.IsSuccess)
        {
            result.Value.RegisteredAt = DateTime.UtcNow;
        }

        return result;
    }

    public Result UpdateDescripiton(RichTextContent newDescripiton)
    {
        if (!Status.IsEditable)
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatus", $"Cannot update description when vacancy is in '{Status}' status."));
        }

        if (!IsValidDescription(newDescripiton))
        {
            return Result.Failure(Error.Problem("Vacancy.InvalidDescription", "Description cannot be null or empty."));
        }

        return UpdateProperty(newDescripiton, desc => Description = desc, nameof(Description));
    }

    public Result UpdateTitle(VacancyTitle newTitle)
    {
        if (!Status.IsEditable)
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatus", $"Cannot update title when vacancy is in '{Status}' status."));
        }

        return UpdateProperty(newTitle, title => Title = title, nameof(Title));
    }

    public Result UpdateSalary(Salary newSalary)
    {
        if (!Status.IsEditable)
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatus", $"Cannot update salary when vacancy is in '{Status}' status."));
        }

        return UpdateProperty(newSalary, sal => Salary = sal, nameof(Salary));
    }

    public Result UpdateLocation(Location newLocation)
    {
        if (!Status.IsEditable)
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatus", $"Cannot update location when vacancy is in '{Status}' status."));
        }

        return UpdateProperty(newLocation, loc => Location = loc, nameof(Location));
    }

    public Result UpdateRecruiterInfo(RecruiterInfo newRecruiterInfo)
    {
        if (!Status.IsEditable)
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatus", $"Cannot update recruiter info when vacancy is in '{Status}' status."));
        }

        return UpdateProperty(newRecruiterInfo, info => RecruiterInfo = info, nameof(RecruiterInfo));
    }

    public Result UpdateCategoryId(CategoryId newCategoryId)
    {
        if (Status != VacancyStatus.Registered)
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatus", $"Category can only be assigned when the vacancy is in '{VacancyStatus.Registered.Name}' status."));
        }

        return UpdateProperty(newCategoryId, catId => CategoryId = catId, nameof(CategoryId));
    }

    public Result Register()
    {
        if (!Status.CanTransitionTo(VacancyStatus.Registered))
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatusTransition", $"Vacancy in '{Status.Name}' status cannot be registered."));
        }

        Status = VacancyStatus.Registered;
        RegisteredAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Publish()
    {
        if (!Status.CanTransitionTo(VacancyStatus.Published))
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatusTransition", $"Vacancy in '{Status.Name}' status cannot be published."));
        }

        if (CategoryId == null)
        {
            return Result.Failure(Error.Conflict("Vacancy.MissingCategory", "Category must be assigned before publishing the vacancy."));
        }

        Status = VacancyStatus.Published;
        PublishedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Archive()
    {
        if (!Status.CanTransitionTo(VacancyStatus.Archived))
        {
            return Result.Failure(Error.Conflict("Vacancy.InvalidStatusTransition", $"Vacancy in '{Status.Name}' status cannot be archived."));
        }

        Status = VacancyStatus.Archived;
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    private static Result<Vacancy> Create(
        VacancyTitle title,
        RichTextContent descripiton,
        VacancyStatus status,
        Salary salary,
        CompanyId companyId,
        Location location,
        RecruiterInfo recruiterInfo)
    {
        var validationResult = ValidateCreationProperties(title, descripiton, salary, companyId, location, recruiterInfo);
        if (validationResult.IsFailure)
        {
            return Result<Vacancy>.Failure(validationResult.Error);
        }

        var vacancy = new Vacancy(title, descripiton, status, salary, companyId, location, recruiterInfo);
        return Result<Vacancy>.Success(vacancy);
    }

    private static Result ValidateCreationProperties(
        VacancyTitle title,
        RichTextContent descripiton,
        Salary salary,
        CompanyId companyId,
        Location location,
        RecruiterInfo recruiterInfo)
    {
        if (title == null)
        {
            return Result.Failure(Error.Problem("Vacancy.InvalidTitle", "Title cannot be null."));
        }

        if (!IsValidDescription(descripiton))
        {
            return Result.Failure(Error.Problem("Vacancy.InvalidDescription", "Description cannot be null or empty."));
        }

        if (salary == null)
        {
            return Result.Failure(Error.Problem("Vacancy.InvalidSalary", "Salary cannot be null."));
        }

        if (companyId == null)
        {
            return Result.Failure(Error.Problem("Vacancy.InvalidCompanyId", "CompanyId cannot be null."));
        }

        if (location == null)
        {
            return Result.Failure(Error.Problem("Vacancy.InvalidLocation", "Location cannot be null."));
        }

        if (recruiterInfo == null)
        {
            return Result.Failure(Error.Problem("Vacancy.InvalidRecruiterInfo", "RecruiterInfo cannot be null."));
        }

        return Result.Success();
    }

    private static bool IsValidDescription(RichTextContent description) => description is not null && !string.IsNullOrWhiteSpace(description.Markdown);

    private Result UpdateProperty<T>(T newValue, Action<T> updateAction, string propertyName)
    {
        if (newValue is null || newValue.Equals(default(T)))
        {
            return Result.Failure(Error.Problem($"Vacancy.Invalid{propertyName}", $"{propertyName} cannot be null or empty."));
        }

        updateAction(newValue);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
