using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Dtos;
using Application.Commands.Resumes.Educations.Add;

namespace API.Endpoints.Resumes.Educations.Add;

internal sealed class AddEducation : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("resumes/{resumeId:guid}/educations", async (
            Guid resumeId,
            EducationDto education,
            ICommandHandler<AddResumeEducationCommand, AddResumeEducationCommandResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddResumeEducationCommand(resumeId, education);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}