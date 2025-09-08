using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.Create;

public record CreateVacancyCommand(
    Guid EmployerId,
    string Title,
    string DescriptionMarkdown,
    decimal MinSalary,
    decimal? MaxSalary,
    string? SalaryCurrency,
    string Country,
    string City,
    string? Region,
    string? District,
    string? Address,
    decimal? Latitude,
    decimal? Longitude,
    string RecruiterFirstName,
    string RecruiterEmail,
    string RecruiterPhoneNumber,
    string RecruiterPhoneNumberRegionCode,
    bool IsDraft) : ICommand<CreateVacancyCommandResponse>;

public record CreateVacancyCommandResponse(Guid Id);
