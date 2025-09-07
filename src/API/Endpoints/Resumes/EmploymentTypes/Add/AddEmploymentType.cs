using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.EmploymentTypes.Add;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.EmploymentTypes.Add;

internal sealed class AddEmploymentType : IEndpoint
{
    internal sealed record AddResumeEmploymentTypeRequest(ICollection<string> EmploymentTypes);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("resumes/{resumeId:guid}/employment-types/add", async (
            Guid resumeId,
            AddResumeEmploymentTypeRequest request,
            ICommandHandler<AddResumeEmploymentTypeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddResumeEmploymentTypeCommand(resumeId, request.EmploymentTypes);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}