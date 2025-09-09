using Application.Abstractions.Messaging;
using Domain.ReadModels;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetVacancyApplications;

internal sealed class GetVacancyApplicationsQueryHandler : IQueryHandler<GetVacancyApplicationsQuery, GetVacancyApplicationsQueryResponse>
{
    private readonly IVacancyApplicationsReadModelRepository _vacancyApplicationsReadModelRepository;
    private readonly IPagedList<VacancyApplicationDto> _pagedList;

    public GetVacancyApplicationsQueryHandler(IVacancyApplicationsReadModelRepository vacancyApplicationsReadModelRepository, IPagedList<VacancyApplicationDto> pagedList)
    {
        _vacancyApplicationsReadModelRepository = vacancyApplicationsReadModelRepository;
        _pagedList = pagedList;
    }

    public async Task<Result<GetVacancyApplicationsQueryResponse>> Handle(GetVacancyApplicationsQuery query, CancellationToken cancellationToken = default)
    {
        var vacancyApplicationsQuery = _vacancyApplicationsReadModelRepository.GetVacancyApplicationsReadModelsQueryable(query.VacancyId);
        var vacancyApplicationsDtos = vacancyApplicationsQuery.Select(x => new VacancyApplicationDto(
            x.VacancyApplicationId,
            x.SeekerFirstName,
            x.SeekerLastName,
            x.CoverLetter,
            x.ResumeId,
            x.ResumeTitle,
            x.FileUrl,
            x.AppliedAt));

        var vacancyApplications = await _pagedList.Create(vacancyApplicationsDtos, query.Page, query.PageSize);
        return Result<GetVacancyApplicationsQueryResponse>.Success(new GetVacancyApplicationsQueryResponse(vacancyApplications));
    }
}
