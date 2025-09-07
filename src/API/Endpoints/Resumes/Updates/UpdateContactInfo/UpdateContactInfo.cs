using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.UpdateContactInfo;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Updates.UpdateContactInfo;

internal sealed class UpdateContactInfo : IEndpoint
{
    internal sealed record UpdateResumeContactInfoRequest(string Email, string PhoneNumber, string PhoneNumberRegionCode);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/resumes/{resumeId:guid}/contact-info", async (
            Guid resumeId,
            UpdateResumeContactInfoRequest request,
            ICommandHandler<UpdateResumeContactInfoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateResumeContactInfoCommand(resumeId, request.Email, request.PhoneNumber, request.PhoneNumberRegionCode);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.NoContent() : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}
