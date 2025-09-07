using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.PublishResume;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Updates.PublishResume;

internal sealed class PublishResume : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/resumes/{resumeId:guid}/publish", async (
            Guid resumeId,
            ICommandHandler<PublishResumeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new PublishResumeCommand(resumeId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
