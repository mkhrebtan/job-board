using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.DeleteResume;

namespace API.Endpoints.Resumes.Delete;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/resumes/{resumeId:guid}", async (
            Guid resumeId,
            ICommandHandler<DeleteResumeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteResumeCommand(resumeId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}
