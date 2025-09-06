using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.UpdateTitle;

namespace API.Endpoints.Vacancies.UpdateTitle;

internal sealed class UpdateTitle : IEndpoint
{
    public record UpdateVacancyTitleRequest(string Title);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/title", async (
            Guid vacancyId,
            UpdateVacancyTitleRequest request,
            ICommandHandler<UpdateVacancyTitleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateVacancyTitleCommand(vacancyId, request.Title);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
