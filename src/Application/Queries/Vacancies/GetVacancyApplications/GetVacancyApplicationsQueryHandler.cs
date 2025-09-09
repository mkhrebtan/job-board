using Application.Abstractions.Messaging;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetVacancyApplications;

internal sealed class GetVacancyApplicationsQueryHandler : IQueryHandler<GetVacancyApplicationsQuery, GetVacancyApplicationsQueryResponse>
{
    private readonly IVacancyApplicationsReadModelRepository _vacancyApplicationsReadModelRepository;

    public GetVacancyApplicationsQueryHandler(IVacancyApplicationsReadModelRepository vacancyApplicationsReadModelRepository)
    {
        _vacancyApplicationsReadModelRepository = vacancyApplicationsReadModelRepository;
    }

    public async Task<Result<GetVacancyApplicationsQueryResponse>> Handle(GetVacancyApplicationsQuery query, CancellationToken cancellationToken = default)
    {
        var vacancyApplications = await _vacancyApplicationsReadModelRepository.GetVacancyApplicationsAsync(query.VacancyId, cancellationToken);
        return Result<GetVacancyApplicationsQueryResponse>.Success(new GetVacancyApplicationsQueryResponse(vacancyApplications.Select(x => new VacancyApplicationDto(
            x.VacancyApplicationId,
            x.SeekerFirstName,
            x.SeekerLastName,
            x.CoverLetter,
            x.ResumeId,
            x.ResumeTitle,
            x.FileUrl,
            x.AppliedAt))));
    }
}
