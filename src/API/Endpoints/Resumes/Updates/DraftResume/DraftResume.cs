
using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.DraftResume;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Updates.DraftResume;

internal sealed class DraftResume : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/resumes/{resumeId:guid}/draft", async (
            Guid resumeId,
            ICommandHandler<DraftResumeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DraftResumeCommand(resumeId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
