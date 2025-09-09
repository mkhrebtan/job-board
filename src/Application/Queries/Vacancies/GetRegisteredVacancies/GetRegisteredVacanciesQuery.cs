using Application.Abstractions.Messaging;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetRegisteredVacancies;

public record GetRegisteredVacanciesQuery() : IQuery<GetRegisteredVacanciesQueryResponse>;

public record GetRegisteredVacanciesQueryResponse(IEnumerable<RegisteredVacancyDto> RegisteredVacancies);

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

    public GetRegisteredVacanciesQueryHandler(IRegisteredVacanciesReadModelRepository registeredVacanciesReadModelRepository)
    {
        _registeredVacanciesReadModelRepository = registeredVacanciesReadModelRepository;
    }

    public async Task<Result<GetRegisteredVacanciesQueryResponse>> Handle(GetRegisteredVacanciesQuery query, CancellationToken cancellationToken = default)
    {
        var vacancies = await _registeredVacanciesReadModelRepository.GetAllAsync(cancellationToken);
        return Result<GetRegisteredVacanciesQueryResponse>.Success(new GetRegisteredVacanciesQueryResponse(vacancies.Select(v => new RegisteredVacancyDto(
            v.VacancyId,
            v.Title,
            v.CompanyName,
            v.UserFullName,
            v.UserEmail,
            v.UserPhoneNumber,
            v.RegisteredAt))));
    }
}
