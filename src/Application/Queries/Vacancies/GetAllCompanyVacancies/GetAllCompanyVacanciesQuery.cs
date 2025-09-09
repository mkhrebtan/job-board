using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetPublishedCompanyVacancies;

namespace Application.Queries.Vacancies.GetAllCompanyVacancies;

public record GetAllCompanyVacanciesQuery(Guid CompanyId) : IQuery<GetAllCompanyVacanciesQueryResponse>;

public record GetAllCompanyVacanciesQueryResponse(IEnumerable<CompanyVacancyDto> Vacancies);