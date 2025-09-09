using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetRegisteredVacancies;

namespace API.Endpoints.Vacancies.GetRegistered;

internal sealed class GetRegistered : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("vacancies/registered", async (
            IQueryHandler<GetRegisteredVacanciesQuery, GetRegisteredVacanciesQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetRegisteredVacanciesQuery(), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        });
    }
}
