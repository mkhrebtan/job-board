using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.WorkArrangements.Add;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.WorkArrangements.Add;

internal sealed class AddWorkArrangement : IEndpoint
{
    internal sealed record AddResumeWorkArrangementRequest(ICollection<string> WorkArrangements);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("resumes/{resumeId:guid}/work-arrangements/add", async (
            Guid resumeId,
            AddResumeWorkArrangementRequest request,
            ICommandHandler<AddResumeWorkArrangementCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddResumeWorkArrangementCommand(resumeId, request.WorkArrangements);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}