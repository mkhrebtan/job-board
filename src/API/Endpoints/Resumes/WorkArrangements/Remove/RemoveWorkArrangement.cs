using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.WorkArrangements.Remove;

namespace API.Endpoints.Resumes.WorkArrangements.Remove;

internal sealed class RemoveWorkArrangement : IEndpoint
{
    internal sealed record RemoveResumeWorkArrangementRequest(ICollection<string> WorkArrangements);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("resumes/{resumeId:guid}/work-arrangements", async (
            Guid resumeId,
            RemoveResumeWorkArrangementRequest request,
            ICommandHandler<RemoveResumeWorkArrangementCommand> handler,
            CancellationToken cancellationToken) =>
            {
                var command = new RemoveResumeWorkArrangementCommand(resumeId, request.WorkArrangements);
                var result = await handler.Handle(command, cancellationToken);
                return result.IsSuccess ? Results.NoContent() : result.GetProblem();
            })
            .WithTags("Resumes");
    }
}
