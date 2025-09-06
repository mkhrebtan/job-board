using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.Publish;

namespace API.Endpoints.Vacancies.PublishVacancy;

internal sealed class Publish : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/publish", async (
            Guid vacancyId,
            ICommandHandler<PublishVacancyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new PublishVacancyCommand(vacancyId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
