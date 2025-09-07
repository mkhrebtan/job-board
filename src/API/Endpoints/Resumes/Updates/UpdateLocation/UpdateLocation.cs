
using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.UpdateLocation;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Updates.UpdateLocation;

internal sealed class UpdateLocation : IEndpoint
{
    internal sealed record UpdateResumeLocationReguest(
        string Country,
        string City,
        string? Region,
        string? District,
        decimal? Latitude,
        decimal? Longitude);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/resumes/{resumeId:guid}/location", async (
            Guid resumeId,
            UpdateResumeLocationReguest request,
            ICommandHandler<UpdateResumeLocationCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateResumeLocationCommand(resumeId, request.Country, request.City, request.Region, request.District, request.Latitude, request.Longitude);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
