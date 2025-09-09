using Application.Abstractions.Messaging;

namespace Application.Queries.Vacancies.GetVacancyApplications;

public record GetVacancyApplicationsQuery(Guid VacancyId) : IQuery<GetVacancyApplicationsQueryResponse>;

public record GetVacancyApplicationsQueryResponse(IEnumerable<VacancyApplicationDto> VacancyApplications);

public record VacancyApplicationDto(
    Guid Id,
    string SeekerFirstName,
    string SeekerLastName,
    string CoverLetter,
    Guid? ResumeId,
    string? ResumeTitle,
    string? FileUrl,
    DateTime AppliedAt);