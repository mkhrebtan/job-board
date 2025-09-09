using Application.Abstractions.Messaging;
using Domain.Contexts.JobPostingContext.Enums;
using Domain.ReadModels;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetPublishedCompanyVacancies;

internal sealed class GetPublishedCompanyVacanciesHandler : IQueryHandler<GetPublishedCompanyVacanciesQuery, GetPublishedCompanyVacanciesQueryResponse>
{
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;
    private readonly IPagedList<CompanyVacancyDto> _pagedList;

    public GetPublishedCompanyVacanciesHandler(ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository, IPagedList<CompanyVacancyDto> pagedList)
    {
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
        _pagedList = pagedList;
    }

    public async Task<Result<GetPublishedCompanyVacanciesQueryResponse>> Handle(GetPublishedCompanyVacanciesQuery query, CancellationToken cancellationToken = default)
    {
        var vacanciesQuery = _companyVacanciesReadModelRepository.GetCompanyVacanciesQueryable(query.CompanyId);
        vacanciesQuery = vacanciesQuery.Where(v => v.Status == VacancyStatus.Published.Code);
        if (query.CategoryId is not null)
        {
            vacanciesQuery = vacanciesQuery.Where(v => v.CategoryId == query.CategoryId);
        }

        if (query.NewFirst == true)
        {
            vacanciesQuery = vacanciesQuery.OrderByDescending(v => v.LastUpdatedAt);
        }
        else
        {
            vacanciesQuery = vacanciesQuery.OrderBy(v => v.LastUpdatedAt);
        }

        var vacanciesDtos = vacanciesQuery.Select(v => new CompanyVacancyDto(
            v.VacancyId,
            v.Title,
            v.SalaryFrom,
            v.SalaryTo,
            v.SalaryCurrency,
            v.Country,
            v.City,
            v.Region,
            v.District,
            v.LastUpdatedAt));
        var vacancies = await _pagedList.Create(vacanciesDtos, query.Page, query.PageSize);
        return Result<GetPublishedCompanyVacanciesQueryResponse>.Success(new GetPublishedCompanyVacanciesQueryResponse(vacancies));
    }
}
