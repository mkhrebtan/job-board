using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Dtos;
using Application.Commands.Resumes.WorkExperiences.Add;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.WorkExperiences.Add;

internal sealed class AddWorkExperience : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("resumes/{resumeId:guid}/work-experiences", async (
            Guid resumeId,
            WorkExperienceDto workExperience,
            ICommandHandler<AddResumeWorkExperienceCommand, AddResumeWorkExperienceCommandResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddResumeWorkExperienceCommand(resumeId, workExperience);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
