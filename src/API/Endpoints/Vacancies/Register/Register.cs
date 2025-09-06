using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.Register;

namespace API.Endpoints.Vacancies.Register;

internal sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/register", async (
            Guid vacancyId,
            ICommandHandler<RegisterVacancyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterVacancyCommand(vacancyId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
