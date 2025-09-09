using Application.Abstractions.Messaging;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetPublishedCompanyVacancies;

internal sealed class GetPublishedCompanyVacanciesHandler : IQueryHandler<GetPublishedCompanyVacanciesQuery, GetPublishedCompanyVacanciesQueryResponse>
{
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;

    public GetPublishedCompanyVacanciesHandler(ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository)
    {
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
    }

    public async Task<Result<GetPublishedCompanyVacanciesQueryResponse>> Handle(GetPublishedCompanyVacanciesQuery query, CancellationToken cancellationToken = default)
    {
        var vacancies = await _companyVacanciesReadModelRepository.GetPublishedByCompanyIdAsync(query.CompanyId, cancellationToken);
        return Result<GetPublishedCompanyVacanciesQueryResponse>.Success(new GetPublishedCompanyVacanciesQueryResponse(vacancies.Select(v => new CompanyVacancyDto(
            v.VacancyId,
            v.Title,
            v.SalaryFrom,
            v.SalaryTo,
            v.SalaryCurrency,
            v.Country,
            v.City,
            v.Region,
            v.District,
            v.LastUpdatedAt))));
    }
}
