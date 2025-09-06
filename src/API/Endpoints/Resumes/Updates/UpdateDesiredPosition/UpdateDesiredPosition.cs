using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.UpdateDesiredPosition;

namespace API.Endpoints.Resumes.Updates.UpdateDesiredPosition;

internal sealed class UpdateDesiredPosition : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/resumes/{resumeId:guid}/desired-position", async (
            Guid resumeId,
            string positionTitle,
            ICommandHandler<UpdateResumeDesiredPositionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateResumeDesiredPositionCommand(resumeId, positionTitle);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}
