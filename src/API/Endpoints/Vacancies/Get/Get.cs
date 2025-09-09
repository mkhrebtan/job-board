using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetVacancy;

namespace API.Endpoints.Vacancies.Get;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/vacancies/{vacancyId:guid}", async (
            Guid vacancyId,
            IQueryHandler<GetVacancyQuery, GetVacancyQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetVacancyQuery(vacancyId), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
