using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetPublishedCompanyVacancies;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetAllCompanyVacancies;

internal sealed class GetAllCompanyVacanciesQueryHandler : IQueryHandler<GetAllCompanyVacanciesQuery, GetAllCompanyVacanciesQueryResponse>
{
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;

    public GetAllCompanyVacanciesQueryHandler(ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository)
    {
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
    }

    public async Task<Result<GetAllCompanyVacanciesQueryResponse>> Handle(GetAllCompanyVacanciesQuery query, CancellationToken cancellationToken = default)
    {
        var vacancies = await _companyVacanciesReadModelRepository.GetAllByCompanyIdAsync(query.CompanyId, cancellationToken);
        return Result<GetAllCompanyVacanciesQueryResponse>.Success(new GetAllCompanyVacanciesQueryResponse(vacancies.Select(v => new CompanyVacancyDto(
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
