using Application.Abstractions.Messaging;
using Domain.ReadModels;

namespace Application.Queries.Vacancies.GetAllVacancies;

public record GetAllVacanciesQuery(
    Guid[]? CategoryIds,
    string? Search,
    string? SortProperty,
    bool? SortDescending,
    decimal? MinSalary,
    decimal? MaxSalary,
    string? SalaryCurrency,
    string? Country,
    string? City,
    string? Region,
    string? District,
    decimal? Latitude,
    decimal? Longitude,
    int Page,
    int PageSize) : IQuery<GetAllVacanciesQueryResponse>;

public record GetAllVacanciesQueryResponse(IPagedList<VacancyDto> Vacancies);

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
    decimal? Latitude,
    decimal? Longitude,
    DateTime LastUpdatedAt);