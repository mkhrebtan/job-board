using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.EmploymentTypes.Remove;

namespace API.Endpoints.Resumes.EmploymentTypes.Remove;

internal sealed class RemoveEmploymentType : IEndpoint
{
    internal sealed record RemoveResumeEmploymentTypeRequest(ICollection<string> EmploymentTypes);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("resumes/{resumeId:guid}/employment-types", async (
            Guid resumeId,
            RemoveResumeEmploymentTypeRequest request,
            ICommandHandler<RemoveResumeEmploymentTypeCommand> handler,
            CancellationToken cancellationToken) =>
            {
                var command = new RemoveResumeEmploymentTypeCommand(resumeId, request.EmploymentTypes);
                var result = await handler.Handle(command, cancellationToken);
                return result.IsSuccess ? Results.NoContent() : result.GetProblem();
            })
            .WithTags("Resumes");
    }
}
