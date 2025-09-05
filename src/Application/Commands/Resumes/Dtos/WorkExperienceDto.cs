namespace Application.Commands.Resumes.Dtos;

public record WorkExperienceDto(
    string CompanyName,
    string Position,
    string DescriptionMarkdown,
    DateTime StartDate,
    DateTime? EndDate);
