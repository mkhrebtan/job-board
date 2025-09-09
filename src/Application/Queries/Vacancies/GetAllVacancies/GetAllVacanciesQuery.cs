using Application.Abstractions.Messaging;

namespace Application.Queries.Vacancies.GetAllVacancies;

public record GetAllVacanciesQuery() : IQuery<GetAllVacanciesQueryResponse>;

public record GetAllVacanciesQueryResponse(IEnumerable<VacancyDto> Vacancies);

public record VacancyDto(
    Guid Id,
    string Title,
    string CompanyName,
    string? CompanyLogoUrl,
    decimal? SalaryFrom,
    decimal? SalaryTo,
    string? SalaryCurrency,
    string Country,
    string City,
    string? Region,
    string? District,
    DateTime LastUpdatedAt);