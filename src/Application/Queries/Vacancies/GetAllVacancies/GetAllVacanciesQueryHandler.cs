using Application.Abstractions.Messaging;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetAllVacancies;

internal sealed class GetAllVacanciesQueryHandler : IQueryHandler<GetAllVacanciesQuery, GetAllVacanciesQueryResponse>
{
    private readonly IVacancyListingReadModelRepository _vacancyListingReadModelRepository;

    public GetAllVacanciesQueryHandler(IVacancyListingReadModelRepository vacancyListingReadModelRepository)
    {
        _vacancyListingReadModelRepository = vacancyListingReadModelRepository;
    }

    public async Task<Result<GetAllVacanciesQueryResponse>> Handle(GetAllVacanciesQuery query, CancellationToken cancellationToken = default)
    {
        var vacancies = await _vacancyListingReadModelRepository.GetAllAsync(cancellationToken);
        return Result<GetAllVacanciesQueryResponse>.Success(new GetAllVacanciesQueryResponse(vacancies.Select(v => new VacancyDto(
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
            v.LastUpdatedAt
        ))));
    }
}
