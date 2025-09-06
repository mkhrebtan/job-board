using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.WorkExperiences.Remove;

namespace API.Endpoints.Resumes.WorkExperiences.Remove;

internal sealed class RemoveWorkExperience : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("resumes/{resumeId:guid}/work-experiences/{workExperienceId:guid}", async (
            Guid resumeId,
            Guid workExperienceId,
            ICommandHandler<RemoveResumeWorkExperienceCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveResumeWorkExperienceCommand(resumeId, workExperienceId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}