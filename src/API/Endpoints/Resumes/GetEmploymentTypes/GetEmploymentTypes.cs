using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Resumes.GetEmploymentTypes;

namespace API.Endpoints.Resumes.GetEmploymentTypes;

internal sealed class GetEmploymentTypes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/resumes/employment-types", async (
            IQueryHandler<GetEmploymentTypesQuery, GetEmploymentTypesQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetEmploymentTypesQuery(), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}
