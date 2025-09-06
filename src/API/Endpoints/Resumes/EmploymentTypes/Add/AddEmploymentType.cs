using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.EmploymentTypes.Add;

namespace API.Endpoints.Resumes.EmploymentTypes.Add;

internal sealed class AddEmploymentType : IEndpoint
{
    internal sealed record AddResumeEmploymentTypeRequest(ICollection<string> EmploymentTypes);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("resumes/{resumeId:guid}/employment-types", async (
            Guid resumeId,
            AddResumeEmploymentTypeRequest request,
            ICommandHandler<AddResumeEmploymentTypeCommand> handler,
            CancellationToken cancellationToken) =>
            {
                var command = new AddResumeEmploymentTypeCommand(resumeId, request.EmploymentTypes);
                var result = await handler.Handle(command, cancellationToken);
                return result.IsSuccess ? Results.NoContent() : result.GetProblem();
            })
            .WithTags("Resumes");
    }
}