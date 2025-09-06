using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.UpdateRectuiterInfo;

namespace API.Endpoints.Vacancies.UpdateRecruiterInfo;

internal sealed class UpdateRecruiterInfo : IEndpoint
{
    public record UpdateVacancyRecruiterInfoRequest(
        string RecruiterFirstName,
        string RecruiterEmail,
        string RecruiterPhoneNumber,
        string RecruiterPhoneNumberRegionCode);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/recruiter", async (
            Guid vacancyId,
            UpdateVacancyRecruiterInfoRequest request,
            ICommandHandler<UpdateVacancyRecruiterInfoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateVacancyRecruiterInfoCommand(
                vacancyId,
                request.RecruiterFirstName,
                request.RecruiterEmail,
                request.RecruiterPhoneNumber,
                request.RecruiterPhoneNumberRegionCode);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
