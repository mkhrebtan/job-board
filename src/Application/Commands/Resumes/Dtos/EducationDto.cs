namespace Application.Commands.Resumes.Dtos;

public record EducationDto(
    string InstitutionName,
    string Degree,
    string FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate);
