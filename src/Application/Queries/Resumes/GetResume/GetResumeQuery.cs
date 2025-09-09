using Application.Abstractions.Messaging;

namespace Application.Queries.Resumes.GetResume;

public record class GetResumeQuery(Guid ResumeId) : IQuery<GetResumeQueryResponse>;

public record class GetResumeQueryResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Country,
    string City,
    string? Region,
    string? District,
    decimal? Latitude,
    decimal? Longitude,
    string Email,
    string PhoneNumber,
    string Title,
    decimal SalaryAmount,
    string SalaryCurrency,
    string SkillsDescriptionMarkdown,
    ICollection<string> EmploymentTypes,
    ICollection<string> WorkArrangements,
    ICollection<WorkExperienceDto> WorkExperiences,
    ICollection<EducationDto> Educations,
    ICollection<LanguageSkillDto> Languages);

public record EducationDto(
    Guid Id,
    string InstitutionName,
    string Degree,
    string FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate);

public record WorkExperienceDto(
    Guid Id,
    string CompanyName,
    string Position,
    string DescriptionMarkdown,
    DateTime StartDate,
    DateTime? EndDate);

public record LanguageSkillDto(
    Guid Id,
    string Language,
    string ProficiencyLevel);