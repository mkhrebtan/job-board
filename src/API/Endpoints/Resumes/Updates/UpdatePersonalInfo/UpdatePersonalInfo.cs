using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.UpdatePersonalInfo;

namespace API.Endpoints.Resumes.Updates.UpdatePersonalInfo;

internal sealed class UpdatePersonalInfo : IEndpoint
{
    internal sealed record UpdateResumePersonalInfoRequest(string FirstName, string LastName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/resumes/{resumeId:guid}/personal-info", async (
            Guid resumeId,
            UpdateResumePersonalInfoRequest request,
            ICommandHandler<UpdateResumePersonalInfoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateResumePersonalInfoCommand(resumeId, request.FirstName, request.LastName);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}
