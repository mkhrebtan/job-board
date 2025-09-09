using Application.Abstractions.Messaging;
using Domain.ReadModels;

namespace Application.Queries.Vacancies.GetVacancyApplications;

public record GetVacancyApplicationsQuery(Guid VacancyId, int Page, int PageSize) : IQuery<GetVacancyApplicationsQueryResponse>;

public record GetVacancyApplicationsQueryResponse(IPagedList<VacancyApplicationDto> VacancyApplications);

public record VacancyApplicationDto(
    Guid Id,
    string SeekerFirstName,
    string SeekerLastName,
    string CoverLetter,
    Guid? ResumeId,
    string? ResumeTitle,
    string? FileUrl,
    DateTime AppliedAt);