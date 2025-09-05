using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Dtos;

namespace Application.Commands.Resumes.Create;

public record CreateResumeCommand(
    Guid SeekerId,
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
    string PhoneNumberRegionCode,
    string DesiredPositionTitle,
    decimal SalaryAmount,
    string SalaryCurrency,
    string SkillsDescripitonMarkdown,
    ICollection<string> EmploymentTypes,
    ICollection<string> WorkArrangements,
    ICollection<WorkExperienceDto> WorkExpetiences,
    ICollection<EducationDto> Educations,
    ICollection<LanguageSkillDto> Languages) : ICommand<CreateResumeCommandResponse>;

public record CreateResumeCommandResponse(Guid ResumeId);