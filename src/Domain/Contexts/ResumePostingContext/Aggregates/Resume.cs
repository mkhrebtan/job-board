using Domain.Abstraction;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Entities;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.ResumePostingContext.Aggregates;

public class Resume : AggregateRoot<ResumeId>
{
    private readonly HashSet<EmploymentType> _employmentTypes;
    private readonly HashSet<WorkArrangement> _workArrangements;
    private readonly Dictionary<WorkExperienceId, WorkExperience> _workExperiences = [];
    private readonly Dictionary<EducationId, Education> _educations = [];
    private readonly Dictionary<LanguageId, LanguageSkill> _languages = [];

    private Resume(
        UserId seekerId,
        PersonalInfo personalInfo,
        Location location,
        ContactInfo contactInfo,
        DesiredPosition desiredPosition,
        Money salary,
        RichTextContent skillsDescription,
        ICollection<EmploymentType>? employmentTypes,
        ICollection<WorkArrangement>? workArrangements)
            : base(new ResumeId())
    {
        SeekerId = seekerId;
        PersonalInfo = personalInfo;
        Location = location;
        ContactInfo = contactInfo;
        DesiredPosition = desiredPosition;
        SalaryExpectation = salary;
        SkillsDescription = skillsDescription;
        _employmentTypes = employmentTypes is not null
            ? [.. employmentTypes]
            : new();
        _workArrangements = workArrangements is not null
            ? [.. workArrangements]
            : new();
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
        Status = ResumeStatus.Draft;
    }

    public UserId SeekerId { get; private init; }

    public PersonalInfo PersonalInfo { get; private set; }

    public Location Location { get; private set; }

    public ContactInfo ContactInfo { get; private set; }

    public DesiredPosition DesiredPosition { get; private set; }

    public Money SalaryExpectation { get; private set; }

    public RichTextContent SkillsDescription { get; private set; }

    public TimeSpan TotalExperience
        => WorkExperiences.Any()
            ? TimeSpan.FromDays(WorkExperiences.Sum(we => we.DateRange.Duration.TotalDays))
            : TimeSpan.Zero;

    public DateTime CreatedAt { get; private init; }

    public DateTime LastUpdatedAt { get; private set; }

    public DateTime? PublishedAt { get; private set; }

    public ResumeStatus Status { get; private set; }

    public IReadOnlyCollection<EmploymentType> EmploymentTypes => _employmentTypes.ToList().AsReadOnly();

    public IReadOnlyCollection<WorkArrangement> WorkArrangements => _workArrangements.ToList().AsReadOnly();

    public IReadOnlyCollection<WorkExperience> WorkExperiences => _workExperiences.Values.ToList().AsReadOnly();

    public IReadOnlyCollection<Education> Educations => _educations.Values.ToList().AsReadOnly();

    public IReadOnlyCollection<LanguageSkill> Languages => _languages.Values.ToList().AsReadOnly();

    public static Result<Resume> Create(
        UserId seekerId,
        PersonalInfo personalInfo,
        Location location,
        ContactInfo contactInfo,
        DesiredPosition desiredPosition,
        Money salary,
        RichTextContent skillsDescription,
        ICollection<EmploymentType>? employmentTypes,
        ICollection<WorkArrangement>? workArrangements)
    {
        var validationResult = ValidateCreationProperties(
            seekerId,
            personalInfo,
            location,
            contactInfo,
            desiredPosition,
            salary,
            skillsDescription);
        if (validationResult.IsFailure)
        {
            return Result<Resume>.Failure(validationResult.Error);
        }

        var resume = new Resume(
            seekerId,
            personalInfo,
            location,
            contactInfo,
            desiredPosition,
            salary,
            skillsDescription,
            employmentTypes,
            workArrangements);
        return Result<Resume>.Success(resume);
    }

    public Result Publish()
    {
        if (Status.CanTransitionTo(ResumeStatus.Published))
        {
            return Result.Failure(new Error("Resume.InvalidStatusTransition", $"Resume in '{Status.Name}' status cannot be published."));
        }

        var updateResult = UpdateProperty(ResumeStatus.Published, status => Status = status, nameof(ResumeStatus.Published));
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        PublishedAt = DateTime.UtcNow;
        return updateResult;
    }

    public Result Draft()
    {
        if (Status.CanTransitionTo(ResumeStatus.Draft))
        {
            return Result.Failure(new Error("Resume.InvalidStatusTransition", $"Resume in '{Status.Name}' status cannot be moved to draft."));
        }

        return UpdateProperty(ResumeStatus.Draft, status => Status = status, nameof(ResumeStatus.Draft));
    }

    public Result UpdatePersonalInfo(PersonalInfo personalInfo)
        => UpdateProperty(personalInfo, info => PersonalInfo = info, nameof(PersonalInfo));

    public Result UpdateLocation(Location location)
        => UpdateProperty(location, loc => Location = loc, nameof(Location));

    public Result UpdateContactInfo(ContactInfo contactInfo)
        => UpdateProperty(contactInfo, info => ContactInfo = info, nameof(ContactInfo));

    public Result UpdateDesiredPosition(DesiredPosition desiredPosition)
        => UpdateProperty(desiredPosition, pos => DesiredPosition = pos, nameof(DesiredPosition));

    public Result UpdateSalaryExpectation(Money salary)
        => UpdateProperty(salary, sal => SalaryExpectation = sal, nameof(SalaryExpectation));

    public Result UpdateSkillsDescription(RichTextContent skillsDescription)
        => UpdateProperty(skillsDescription, desc => SkillsDescription = desc, nameof(SkillsDescription));

    public Result AddEmploymentType(EmploymentType employmentType)
    {
        if (employmentType is null)
        {
            return Result.Failure(new Error("Resume.InvalidEmploymentType", "EmploymentType cannot be null."));
        }

        if (_employmentTypes.Contains(employmentType))
        {
            return Result.Failure(new Error("Resume.DuplicateEmploymentType", "EmploymentType already exists."));
        }

        _employmentTypes.Add(employmentType);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveEmploymentType(EmploymentType employmentType)
    {
        if (employmentType is null)
        {
            return Result.Failure(new Error("Resume.InvalidEmploymentType", "EmploymentType cannot be null."));
        }

        if (!_employmentTypes.Contains(employmentType))
        {
            return Result.Failure(new Error("Resume.EmploymentTypeNotFound", "EmploymentType not found."));
        }

        _employmentTypes.Remove(employmentType);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AddWorkArrangement(WorkArrangement workArrangement)
    {
        if (workArrangement is null)
        {
            return Result.Failure(new Error("Resume.InvalidWorkArrangement", "WorkArrangement cannot be null."));
        }

        if (_workArrangements.Contains(workArrangement))
        {
            return Result.Failure(new Error("Resume.DuplicateWorkArrangement", "WorkArrangement already exists."));
        }

        _workArrangements.Add(workArrangement);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveWorkArrangement(WorkArrangement workArrangement)
    {
        if (workArrangement is null)
        {
            return Result.Failure(new Error("Resume.InvalidWorkArrangement", "WorkArrangement cannot be null."));
        }

        if (!_workArrangements.Contains(workArrangement))
        {
            return Result.Failure(new Error("Resume.WorkArrangementNotFound", "WorkArrangement not found."));
        }

        _workArrangements.Remove(workArrangement);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AddWorkExperience(string companyName, string position, DateRange dateRange, RichTextContent description)
    {
        var creationResult = WorkExperience.Create(companyName, position, dateRange, description);
        if (creationResult.IsFailure)
        {
            return Result.Failure(creationResult.Error);
        }

        _workExperiences.Add(creationResult.Value.Id, creationResult.Value);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveWorkExperience(WorkExperienceId workExperienceId)
    {
        if (workExperienceId is null || workExperienceId.Value == Guid.Empty)
        {
            return Result.Failure(new Error("Resume.InvalidWorkExperienceId", "WorkExperienceId cannot be null or empty."));
        }

        if (!_workExperiences.ContainsKey(workExperienceId))
        {
            return Result.Failure(new Error("Resume.WorkExperienceNotFound", "WorkExperience not found."));
        }

        _workExperiences.Remove(workExperienceId);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AddEducation(string institutionName, string degree, string fieldOfStudy, DateRange dateRange)
    {
        var creationResult = Education.Create(institutionName, degree, fieldOfStudy, dateRange);
        if (creationResult.IsFailure)
        {
            return Result.Failure(creationResult.Error);
        }

        _educations.Add(creationResult.Value.Id, creationResult.Value);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveEducation(EducationId educationId)
    {
        if (educationId is null || educationId.Value == Guid.Empty)
        {
            return Result.Failure(new Error("Resume.InvalidEducationId", "EducationId cannot be null or empty."));
        }

        if (!_educations.ContainsKey(educationId))
        {
            return Result.Failure(new Error("Resume.EducationNotFound", "Education not found."));
        }

        _educations.Remove(educationId);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AddLanguage(string name, LanguageLevel proficiency)
    {
        var creationResult = LanguageSkill.Create(name, proficiency);
        if (creationResult.IsFailure)
        {
            return Result.Failure(creationResult.Error);
        }

        if (_languages.Values.Any(lang => lang.Language.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            return Result.Failure(new Error("Resume.DuplicateLanguage", "Language already exists."));
        }

        _languages.Add(creationResult.Value.Id, creationResult.Value);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveLanguage(LanguageId languageId)
    {
        if (languageId is null || languageId.Value == Guid.Empty)
        {
            return Result.Failure(new Error("Resume.InvalidLanguageId", "LanguageId cannot be null or empty."));
        }

        if (!_languages.ContainsKey(languageId))
        {
            return Result.Failure(new Error("Resume.LanguageNotFound", "Language not found."));
        }

        _languages.Remove(languageId);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    private static Result ValidateCreationProperties(
        UserId seekerId,
        PersonalInfo personalInfo,
        Location location,
        ContactInfo contactInfo,
        DesiredPosition desiredPosition,
        Money salary,
        RichTextContent skillsDescription)
    {
        if (seekerId == null || seekerId.Value == Guid.Empty)
        {
            return Result.Failure(new Error("Resume.InvalidSeekerId", "SeekerId cannot be null or empty."));
        }

        if (personalInfo == null)
        {
            return Result.Failure(new Error("Resume.InvalidPersonalInfo", "PersonalInfo cannot be null."));
        }

        if (location == null)
        {
            return Result.Failure(new Error("Resume.InvalidLocation", "Location cannot be null."));
        }

        if (contactInfo == null)
        {
            return Result.Failure(new Error("Resume.InvalidContactInfo", "ContactInfo cannot be null."));
        }

        if (desiredPosition == null)
        {
            return Result.Failure(new Error("Resume.InvalidDesiredPosition", "DesiredPosition cannot be null."));
        }

        if (salary == null)
        {
            return Result.Failure(new Error("Resume.InvalidSalaryExpectation", "SalaryExpectation cannot be null."));
        }

        if (skillsDescription == null)
        {
            return Result.Failure(new Error("Resume.InvalidSkillsDescription", "SkillsDescription cannot be null."));
        }

        return Result.Success();
    }

    private Result UpdateProperty<T>(T newValue, Action<T> updateAction, string propertyName)
    {
        if (newValue is null || newValue.Equals(default(T)))
        {
            return Result.Failure(new Error($"Resume.Invalid{propertyName}", $"{propertyName} cannot be null or empty."));
        }

        updateAction(newValue);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
