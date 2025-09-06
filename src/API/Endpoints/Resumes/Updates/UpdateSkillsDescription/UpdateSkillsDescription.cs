using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.UpdateSkillsDescription;

namespace API.Endpoints.Resumes.Updates.UpdateSkillsDescription;

internal sealed class UpdateSkillsDescription : IEndpoint
{
    public record UpdateResumeSkillsDescriptionRequest(
        string SkillsDescriptionMarkdown);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/resumes/{resumeId:guid}/skills-description", async (
            Guid resumeId,
            UpdateResumeSkillsDescriptionRequest request,
            ICommandHandler<UpdateResumeSkillsDescriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateResumeSkillsDescriptionCommand(resumeId, request.SkillsDescriptionMarkdown);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}
