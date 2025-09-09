using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Resumes.GetResume;

namespace API.Endpoints.Resumes.Get;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/resumes/{resumeId:guid}", async (
            Guid resumeId,
            IQueryHandler<GetResumeQuery, GetResumeQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetResumeQuery(resumeId), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}
