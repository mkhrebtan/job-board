using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Dtos;
using Application.Commands.Resumes.Languages.Add;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Languages.Add;

internal sealed class AddLanguage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("resumes/{resumeId:guid}/languages", async (
            Guid resumeId,
            LanguageSkillDto language,
            ICommandHandler<AddResumeLanguageCommand, AddResumeLanguageCommandResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddResumeLanguageCommand(resumeId, language);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
