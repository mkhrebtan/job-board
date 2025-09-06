using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.UpdateDescription;

namespace API.Endpoints.Vacancies.UpdateDescription;

internal sealed class UpdateDescription : IEndpoint
{
    public record UpdateVacancyDescriptionRequest(string DescriptionMarkdown);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/description", async (
            Guid vacancyId,
            UpdateVacancyDescriptionRequest request,
            ICommandHandler<UpdateVacancyDescriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateVacancyDescriptionCommand(vacancyId, request.DescriptionMarkdown);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
