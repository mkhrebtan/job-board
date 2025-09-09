using Application.Abstractions.Messaging;
using Domain.ReadModels;

namespace Application.Queries.Vacancies.GetPublishedCompanyVacancies;

public record GetPublishedCompanyVacanciesQuery(Guid CompanyId, Guid? CategoryId, bool? NewFirst, int Page, int PageSize) : IQuery<GetPublishedCompanyVacanciesQueryResponse>;

public record GetPublishedCompanyVacanciesQueryResponse(IPagedList<CompanyVacancyDto> Vacancies);

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