using Application.Abstractions.Messaging;

namespace Application.Queries.Vacancies.GetVacancy;

public record GetVacancyQuery(Guid VacancyId) : IQuery<GetVacancyQueryResponse>;

public record GetVacancyQueryResponse(
    Guid Id,
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
    string Status);