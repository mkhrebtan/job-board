using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.VacancyApplications.ApplyWithFile;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.VacancyApplications.ApplyWithFile;

internal sealed class ApplyWithFile : IEndpoint
{
    internal sealed record ApplyToVacancyWithFileRequest(Guid VacancyId, string? CoverLetterContent, string FileUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("vacancies/{vacancyId:guid}/apply-with-file", async (
            Guid vacancyId,
            ApplyToVacancyWithFileRequest request,
            ICommandHandler<ApplyToVacancyWithFileCommand, ApplyToVacancyCommandResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ApplyToVacancyWithFileCommand(Guid.NewGuid(), vacancyId, request.CoverLetterContent, request.FileUrl);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancy Applications")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
