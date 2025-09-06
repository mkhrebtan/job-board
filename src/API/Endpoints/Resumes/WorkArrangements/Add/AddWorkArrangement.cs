using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.WorkArrangements.Add;

namespace API.Endpoints.Resumes.WorkArrangements.Add;

internal sealed class AddWorkArrangement : IEndpoint
{
    internal sealed record AddResumeWorkArrangementRequest(ICollection<string> WorkArrangements);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("resumes/{resumeId:guid}/work-arrangements", async (
            Guid resumeId,
            AddResumeWorkArrangementRequest request,
            ICommandHandler<AddResumeWorkArrangementCommand> handler,
            CancellationToken cancellationToken) =>
            {
                var command = new AddResumeWorkArrangementCommand(resumeId, request.WorkArrangements);
                var result = await handler.Handle(command, cancellationToken);
                return result.IsSuccess ? Results.NoContent() : result.GetProblem();
            })
            .WithTags("Resumes");
    }
}