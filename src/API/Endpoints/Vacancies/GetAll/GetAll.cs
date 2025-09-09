
using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetAllVacancies;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Vacancies.GetAll;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("vacancies/", async (
            IQueryHandler<GetAllVacanciesQuery, GetAllVacanciesQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetAllVacanciesQuery(), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
