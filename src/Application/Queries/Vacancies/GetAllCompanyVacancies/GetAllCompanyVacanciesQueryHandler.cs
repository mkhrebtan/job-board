using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetPublishedCompanyVacancies;
using Domain.ReadModels;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetAllCompanyVacancies;

internal sealed class GetAllCompanyVacanciesQueryHandler : IQueryHandler<GetAllCompanyVacanciesQuery, GetAllCompanyVacanciesQueryResponse>
{
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;
    private readonly IPagedList<CompanyVacancyDto> _pagedList;

    public GetAllCompanyVacanciesQueryHandler(ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository, IPagedList<CompanyVacancyDto> pagedList)
    {
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
        _pagedList = pagedList;
    }

    public async Task<Result<GetAllCompanyVacanciesQueryResponse>> Handle(GetAllCompanyVacanciesQuery query, CancellationToken cancellationToken = default)
    {
        var vacanciesQuery = _companyVacanciesReadModelRepository.GetCompanyVacanciesQueryable(query.CompanyId);
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
        return Result<GetAllCompanyVacanciesQueryResponse>.Success(new GetAllCompanyVacanciesQueryResponse(vacancies));
    }
}
