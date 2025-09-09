using System.Linq.Expressions;
using Application.Abstractions.Messaging;
using Domain.ReadModels;
using Domain.ReadModels.Vacancies;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetAllVacancies;

internal sealed class GetAllVacanciesQueryHandler : IQueryHandler<GetAllVacanciesQuery, GetAllVacanciesQueryResponse>
{
    private readonly IVacancyListingReadModelRepository _vacancyListingReadModelRepository;
    private readonly IPagedList<VacancyDto> _pagedList;

    public GetAllVacanciesQueryHandler(IVacancyListingReadModelRepository vacancyListingReadModelRepository, IPagedList<VacancyDto> pagedList)
    {
        _vacancyListingReadModelRepository = vacancyListingReadModelRepository;
        _pagedList = pagedList;
    }

    public async Task<Result<GetAllVacanciesQueryResponse>> Handle(GetAllVacanciesQuery query, CancellationToken cancellationToken = default)
    {
        var vacanciesQuery = _vacancyListingReadModelRepository.GetVacanciesQueryable();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            vacanciesQuery = vacanciesQuery
                .Where(r =>
                    r.Title.Contains(query.Search) ||
                    r.CompanyName.Contains(query.Search) ||
                    r.DescriptionPlainText.Contains(query.Search));
        }

        Expression<Func<VacancyListingReadModel, object>> sortExpression = query.SortProperty?.ToLower() switch
        {
            "salaryfrom" => r => r.SalaryFrom ?? 0m,
            "salaryto" => r => r.SalaryTo ?? 0m,
            "lastupdatedat" => r => r.LastUpdatedAt,
            _ => r => r.VacancyId,
        };

        if (query.SortDescending == true)
        {
            vacanciesQuery = vacanciesQuery.OrderByDescending(sortExpression);
        }
        else
        {
            vacanciesQuery = vacanciesQuery.OrderBy(sortExpression);
        }

        if (query.MinSalary.HasValue)
        {
            if (string.IsNullOrEmpty(query.SalaryCurrency))
            {
                return Result<GetAllVacanciesQueryResponse>.Failure(Error.Problem("GetVacancies.EmptyCurrency", "Currency must be provided when filtering salary."));
            }

            vacanciesQuery = vacanciesQuery.Where(r => r.SalaryFrom >= query.MinSalary.Value && r.SalaryCurrency == query.SalaryCurrency);
        }

        if (query.MaxSalary.HasValue)
        {
            if (string.IsNullOrEmpty(query.SalaryCurrency))
            {
                return Result<GetAllVacanciesQueryResponse>.Failure(Error.Problem("GetVacancies.EmptyCurrency", "Currency must be provided when filtering salary."));
            }

            vacanciesQuery = vacanciesQuery.Where(r => r.SalaryTo <= query.MaxSalary.Value && r.SalaryCurrency == query.SalaryCurrency);
        }

        if (!string.IsNullOrWhiteSpace(query.Country))
        {
            vacanciesQuery = vacanciesQuery.Where(r => r.Country == query.Country);

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                IQueryable<VacancyListingReadModel>? vacanciesAditionalCities = default!;
                if (query.Latitude.HasValue && query.Longitude.HasValue)
                {
                    const decimal radius = 0.2m;
                    vacanciesAditionalCities = vacanciesQuery.Where(r =>
                        r.Latitude.HasValue && r.Longitude.HasValue &&
                        Math.Abs(r.Latitude.Value - query.Latitude.Value) <= radius &&
                        Math.Abs(r.Longitude.Value - query.Longitude.Value) <= radius);
                }

                vacanciesQuery = vacanciesQuery.Where(r => r.City == query.City).Union(vacanciesAditionalCities);

                if (!string.IsNullOrWhiteSpace(query.Region))
                {
                    vacanciesQuery = vacanciesQuery.Where(r => r.Region == query.Region);

                    if (!string.IsNullOrWhiteSpace(query.District))
                    {
                        vacanciesQuery = vacanciesQuery.Where(r => r.District == query.District);
                    }
                }
            }
        }

        var vacanciesDtos = vacanciesQuery.Select(v => new VacancyDto(
            v.VacancyId,
            v.Title,
            v.CompanyName,
            v.CompanyLogoUrl,
            v.SalaryFrom,
            v.SalaryTo,
            v.SalaryCurrency,
            v.Country,
            v.City,
            v.Region,
            v.District,
            v.Latitude,
            v.Longitude,
            v.LastUpdatedAt));

        var vacanciesPagedList = await _pagedList.Create(vacanciesDtos, query.Page, query.PageSize);
        return Result<GetAllVacanciesQueryResponse>.Success(new GetAllVacanciesQueryResponse(vacanciesPagedList));
    }
}
