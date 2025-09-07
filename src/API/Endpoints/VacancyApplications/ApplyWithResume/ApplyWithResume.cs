using API.Authentication;
using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.VacancyApplications.ApplyWithFile;
using Application.Commands.VacancyApplications.ApplyWithResume;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.VacancyApplications.ApplyWithResume;

internal sealed class ApplyWithResume : IEndpoint
{
    internal sealed record ApplyToVacancyWithResumeRequest(string? CoverLetterContent);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("vacancies/{vacancyId:guid}/apply-with-resume/{resumeId:guid}", async (
            Guid vacancyId,
            Guid resumeId,
            ApplyToVacancyWithResumeRequest request,
            ICommandHandler<ApplyToVacancyWithResumeCommand, ApplyToVacancyCommandResponse> handler,
            IUserContext userContext,
            CancellationToken cancellationToken) =>
        {
            var command = new ApplyToVacancyWithResumeCommand(userContext.UserId, vacancyId, request.CoverLetterContent, resumeId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancy Applications")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
