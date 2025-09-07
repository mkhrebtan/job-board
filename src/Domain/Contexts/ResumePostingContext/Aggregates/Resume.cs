using Domain.Abstraction;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
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

    private readonly HashSet<WorkExperience> _workExperiences = [];
    private readonly HashSet<Education> _educations = [];
    private readonly HashSet<LanguageSkill> _languages = [];

    private Resume()
        : base(new ResumeId())
    {
    }

    private Resume(
        UserId seekerId,
        PersonalInfo personalInfo,
        Location location,
        ContactInfo contactInfo,
        DesiredPosition desiredPosition,
        Money salary,
        RichTextContent skillsDescription,
        ICollection<EmploymentType> employmentTypes,
        ICollection<WorkArrangement> workArrangements)
            : base(new ResumeId())
    {
        SeekerId = seekerId;
        PersonalInfo = personalInfo;
        Location = location;
        ContactInfo = contactInfo;
        DesiredPosition = desiredPosition;
        SalaryExpectation = salary;
        SkillsDescription = skillsDescription;
        _employmentTypes = [.. employmentTypes];
        _workArrangements = [.. workArrangements];
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

    public IReadOnlyCollection<EmploymentType> EmploymentTypes => _employmentTypes;

    public IReadOnlyCollection<WorkArrangement> WorkArrangements => _workArrangements;

    public IReadOnlyCollection<WorkExperience> WorkExperiences => _workExperiences;

    public IReadOnlyCollection<Education> Educations => _educations;

    public IReadOnlyCollection<LanguageSkill> Languages => _languages;

    public Result Publish()
    {
        if (!Status.CanTransitionTo(ResumeStatus.Published))
        {
            return Result.Failure(Error.Conflict("Resume.InvalidStatusTransition", $"Resume in '{Status.Name}' status cannot be published."));
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
        if (!Status.CanTransitionTo(ResumeStatus.Draft))
        {
            return Result.Failure(Error.Conflict("Resume.InvalidStatusTransition", $"Resume in '{Status.Name}' status cannot be moved to draft."));
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
            return Result.Failure(Error.Problem("Resume.InvalidEmploymentType", "EmploymentType cannot be null."));
        }

        if (_employmentTypes.Contains(employmentType))
        {
            return Result.Failure(Error.Problem("Resume.DuplicateEmploymentType", "EmploymentType already exists."));
        }

        _employmentTypes.Add(employmentType);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveEmploymentType(EmploymentType employmentType)
    {
        if (employmentType is null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidEmploymentType", "EmploymentType cannot be null."));
        }

        if (!_employmentTypes.Contains(employmentType))
        {
            return Result.Failure(Error.Problem("Resume.EmploymentTypeNotFound", "EmploymentType not found."));
        }

        if (_employmentTypes.Count == 1)
        {
            return Result.Failure(Error.Problem("Resume.AtLeastOneEmploymentTypeRequired", "At least one EmploymentType must be specified."));
        }

        _employmentTypes.Remove(employmentType);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AddWorkArrangement(WorkArrangement workArrangement)
    {
        if (workArrangement is null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidWorkArrangement", "WorkArrangement cannot be null."));
        }

        if (_workArrangements.Contains(workArrangement))
        {
            return Result.Failure(Error.Problem("Resume.DuplicateWorkArrangement", "WorkArrangement already exists."));
        }

        _workArrangements.Add(workArrangement);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveWorkArrangement(WorkArrangement workArrangement)
    {
        if (workArrangement is null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidWorkArrangement", "WorkArrangement cannot be null."));
        }

        if (!_workArrangements.Contains(workArrangement))
        {
            return Result.Failure(Error.Problem("Resume.WorkArrangementNotFound", "WorkArrangement not found."));
        }

        if (_workArrangements.Count == 1)
        {
            return Result.Failure(Error.Problem("Resume.AtLeastOneWorkArrangementRequired", "At least one WorkArrangement must be specified."));
        }

        _workArrangements.Remove(workArrangement);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result<WorkExperienceId> AddWorkExperience(string companyName, string position, DateRange dateRange, RichTextContent description)
    {
        var creationResult = WorkExperience.Create(Id, companyName, position, dateRange, description);
        if (creationResult.IsFailure)
        {
            return Result<WorkExperienceId>.Failure(creationResult.Error);
        }

        _workExperiences.Add(creationResult.Value);
        LastUpdatedAt = DateTime.UtcNow;
        return Result<WorkExperienceId>.Success(creationResult.Value.Id);
    }

    public Result RemoveWorkExperience(WorkExperienceId workExperienceId)
    {
        if (workExperienceId is null || workExperienceId.Value == Guid.Empty)
        {
            return Result.Failure(Error.Problem("Resume.InvalidWorkExperienceId", "WorkExperienceId cannot be null or empty."));
        }

        var workExperience = _workExperiences.FirstOrDefault(we => we.Id == workExperienceId);
        if (workExperience is null)
        {
            return Result.Failure(Error.Problem("Resume.WorkExperienceNotFound", "Work experience not found."));
        }

        _workExperiences.Remove(workExperience);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result<EducationId> AddEducation(string institutionName, string degree, string fieldOfStudy, DateRange dateRange)
    {
        var creationResult = Education.Create(Id, institutionName, degree, fieldOfStudy, dateRange);
        if (creationResult.IsFailure)
        {
            return Result<EducationId>.Failure(creationResult.Error);
        }

        _educations.Add(creationResult.Value);
        LastUpdatedAt = DateTime.UtcNow;
        return Result<EducationId>.Success(creationResult.Value.Id);
    }

    public Result RemoveEducation(EducationId educationId)
    {
        if (educationId is null || educationId.Value == Guid.Empty)
        {
            return Result.Failure(Error.Problem("Resume.InvalidEducationId", "EducationId cannot be null or empty."));
        }

        var education = _educations.FirstOrDefault(ed => ed.Id == educationId);
        if (education is null)
        {
            return Result.Failure(Error.Problem("Resume.EducationNotFound", "Education not found."));
        }

        _educations.Remove(education);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result<LanguageId> AddLanguage(string name, LanguageLevel proficiency)
    {
        var creationResult = LanguageSkill.Create(Id, name, proficiency);
        if (creationResult.IsFailure)
        {
            return Result<LanguageId>.Failure(creationResult.Error);
        }

        if (_languages.Any(lang => lang.Language.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            return Result<LanguageId>.Failure(Error.Problem("Resume.DuplicateLanguage", "Language already exists."));
        }

        _languages.Add(creationResult.Value);
        LastUpdatedAt = DateTime.UtcNow;
        return Result<LanguageId>.Success(creationResult.Value.Id);
    }

    public Result RemoveLanguage(LanguageId languageId)
    {
        if (languageId is null || languageId.Value == Guid.Empty)
        {
            return Result.Failure(Error.Problem("Resume.InvalidLanguageId", "LanguageId cannot be null or empty."));
        }

        var language = _languages.FirstOrDefault(lang => lang.Id == languageId);
        if (language is null)
        {
            return Result.Failure(Error.Problem("Resume.LanguageNotFound", "Language not found."));
        }

        _languages.Remove(language);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    internal static Result<Resume> Create(
        UserId seekerId,
        PersonalInfo personalInfo,
        Location location,
        ContactInfo contactInfo,
        DesiredPosition desiredPosition,
        Money salary,
        RichTextContent skillsDescription,
        ICollection<EmploymentType> employmentTypes,
        ICollection<WorkArrangement> workArrangements)
    {
        var validationResult = ValidateCreationProperties(
            seekerId,
            personalInfo,
            location,
            contactInfo,
            desiredPosition,
            salary,
            skillsDescription,
            employmentTypes,
            workArrangements);
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

    private static Result ValidateCreationProperties(
        UserId seekerId,
        PersonalInfo personalInfo,
        Location location,
        ContactInfo contactInfo,
        DesiredPosition desiredPosition,
        Money salary,
        RichTextContent skillsDescription,
        ICollection<EmploymentType> employmentTypes,
        ICollection<WorkArrangement> workArrangements)
    {
        if (seekerId == null || seekerId.Value == Guid.Empty)
        {
            return Result.Failure(Error.Problem("Resume.InvalidSeekerId", "SeekerId cannot be null or empty."));
        }

        if (personalInfo == null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidPersonalInfo", "PersonalInfo cannot be null."));
        }

        if (location == null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidLocation", "Location cannot be null."));
        }

        if (contactInfo == null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidContactInfo", "ContactInfo cannot be null."));
        }

        if (desiredPosition == null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidDesiredPosition", "DesiredPosition cannot be null."));
        }

        if (salary == null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidSalaryExpectation", "SalaryExpectation cannot be null."));
        }

        if (skillsDescription == null)
        {
            return Result.Failure(Error.Problem("Resume.InvalidSkillsDescription", "SkillsDescription cannot be null."));
        }

        if (!employmentTypes.Any())
        {
            return Result.Failure(Error.Problem("Resume.InvalidEmploymentTypes", "At least one EmploymentType must be specified."));
        }

        if (!workArrangements.Any())
        {
            return Result.Failure(Error.Problem("Resume.InvalidWorkArrangements", "At least one WorkArrangement must be specified."));
        }

        return Result.Success();
    }

    private Result UpdateProperty<T>(T newValue, Action<T> updateAction, string propertyName)
    {
        if (newValue is null || newValue.Equals(default(T)))
        {
            return Result.Failure(Error.Problem($"Resume.Invalid{propertyName}", $"{propertyName} cannot be null or empty."));
        }

        updateAction(newValue);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
