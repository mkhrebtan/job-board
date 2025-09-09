using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Resumes.GetWorkArrangements;

namespace API.Endpoints.Resumes.GetWorkArrangements;

internal sealed class GetWorkArrangements : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/resumes/work-arrangements", async (
            IQueryHandler<GetWorkArrangementsQuery, GetWorkArrangementsQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetWorkArrangementsQuery(), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}
