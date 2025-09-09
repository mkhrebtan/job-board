using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetPublishedCompanyVacancies;
using Domain.ReadModels;

namespace Application.Queries.Vacancies.GetAllCompanyVacancies;

public record GetAllCompanyVacanciesQuery(Guid CompanyId, int Page, int PageSize) : IQuery<GetAllCompanyVacanciesQueryResponse>;

public record GetAllCompanyVacanciesQueryResponse(IPagedList<CompanyVacancyDto> Vacancies);