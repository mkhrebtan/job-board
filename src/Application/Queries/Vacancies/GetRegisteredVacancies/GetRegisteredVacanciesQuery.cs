using Application.Abstractions.Messaging;
using Domain.ReadModels;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetRegisteredVacancies;

public record GetRegisteredVacanciesQuery(int Page, int PageSize) : IQuery<GetRegisteredVacanciesQueryResponse>;

public record GetRegisteredVacanciesQueryResponse(IPagedList<RegisteredVacancyDto> RegisteredVacancies);

public record RegisteredVacancyDto(
    Guid Id,
    string Title,
    string CompanyName,
    string UserFullName,
    string UserEmail,
    string UserPhoneNumber,
    DateTime RegisteredAt);

internal sealed class GetRegisteredVacanciesQueryHandler : IQueryHandler<GetRegisteredVacanciesQuery, GetRegisteredVacanciesQueryResponse>
{
    private readonly IRegisteredVacanciesReadModelRepository _registeredVacanciesReadModelRepository;
    private readonly IPagedList<RegisteredVacancyDto> _pagedList;

    public GetRegisteredVacanciesQueryHandler(IRegisteredVacanciesReadModelRepository registeredVacanciesReadModelRepository, IPagedList<RegisteredVacancyDto> pagedList)
    {
        _registeredVacanciesReadModelRepository = registeredVacanciesReadModelRepository;
        _pagedList = pagedList;
    }

    public async Task<Result<GetRegisteredVacanciesQueryResponse>> Handle(GetRegisteredVacanciesQuery query, CancellationToken cancellationToken = default)
    {
        var vacanciesQuery = _registeredVacanciesReadModelRepository.GetRegisteredVacanciesQueryable();
        var vacanciesDtos = vacanciesQuery.Select(v => new RegisteredVacancyDto(
            v.VacancyId,
            v.Title,
            v.CompanyName,
            v.UserFullName,
            v.UserEmail,
            v.UserPhoneNumber,
            v.RegisteredAt));

        var vacancies = await _pagedList.Create(vacanciesDtos, query.Page, query.PageSize);
        return Result<GetRegisteredVacanciesQueryResponse>.Success(new GetRegisteredVacanciesQueryResponse(vacancies));
    }
}
