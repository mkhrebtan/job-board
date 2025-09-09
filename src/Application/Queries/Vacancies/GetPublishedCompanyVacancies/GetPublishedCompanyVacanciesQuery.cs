using Application.Abstractions.Messaging;

namespace Application.Queries.Vacancies.GetPublishedCompanyVacancies;

public record GetPublishedCompanyVacanciesQuery(Guid CompanyId) : IQuery<GetPublishedCompanyVacanciesQueryResponse>;

public record GetPublishedCompanyVacanciesQueryResponse(IEnumerable<CompanyVacancyDto> Vacancies);

public record CompanyVacancyDto(
    Guid Id,
    string Title,
    decimal? SalaryFrom,
    decimal? SalaryTo,
    string? SalaryCurrency,
    string Country,
    string City,
    string? Region,
    string? District,
    DateTime LastUpdatedAt);