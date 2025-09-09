using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Resumes.GetUserResumes;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.GetUserResumes;

internal sealed class GetUserResumes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/resumes/{userId}", async (Guid userId, IQueryHandler<GetUserResumesQuery, GetUserResumesResponse> queryHandler) =>
        {
            var query = new GetUserResumesQuery(userId);
            var result = await queryHandler.Handle(query);

            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
