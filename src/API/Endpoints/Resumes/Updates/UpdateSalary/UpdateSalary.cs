
using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.UpdateSalary;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Updates.UpdateSalary;

internal sealed class UpdateSalary : IEndpoint
{
    internal sealed record UpdateResumeSalaryRequest(decimal SalaryAmount, string SalaryCurrency);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/resumes/{resumeId:guid}/salary", async (
            Guid resumeId,
            UpdateResumeSalaryRequest request,
            ICommandHandler<UpdateResumeSalaryCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateResumeSalaryCommand(resumeId, request.SalaryAmount, request.SalaryCurrency);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
