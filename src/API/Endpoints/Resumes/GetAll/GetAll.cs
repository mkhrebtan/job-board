using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Resumes.GetAll;

namespace API.Endpoints.Resumes.GetAll;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("resumes/", async (
            IQueryHandler<GetAllResumesQuery, GetAllResumesQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllResumesQuery();
            var result = await queryHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}
