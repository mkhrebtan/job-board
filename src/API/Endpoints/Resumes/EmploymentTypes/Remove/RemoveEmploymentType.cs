using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.EmploymentTypes.Remove;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.EmploymentTypes.Remove;

internal sealed class RemoveEmploymentType : IEndpoint
{
    internal sealed record RemoveResumeEmploymentTypeRequest(ICollection<string> EmploymentTypes);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("resumes/{resumeId:guid}/employment-types/remove", async (
            Guid resumeId,
            RemoveResumeEmploymentTypeRequest request,
            ICommandHandler<RemoveResumeEmploymentTypeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveResumeEmploymentTypeCommand(resumeId, request.EmploymentTypes);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
