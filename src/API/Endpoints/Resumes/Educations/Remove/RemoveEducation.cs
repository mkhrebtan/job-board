using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Educations.Remove;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Educations.Remove;

internal sealed class RemoveEducation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("resumes/{resumeId:guid}/educations/{educationId:guid}", async (
            Guid resumeId,
            Guid educationId,
            ICommandHandler<RemoveResumeEducationCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveResumeEducationCommand(resumeId, educationId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
