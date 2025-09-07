using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Languages.Remove;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Languages.Remove;

internal sealed class RemoveLanguage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("resumes/{resumeId:guid}/languages/{languageId:guid}", async (
            Guid resumeId,
            Guid languageId,
            ICommandHandler<RemoveResumeLanguageCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveResumeLanguageCommand(resumeId, languageId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}