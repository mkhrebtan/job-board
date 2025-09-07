using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.Archive;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Vacancies.Archive;

internal sealed class Archive : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/archive", async (
            Guid vacancyId,
            ICommandHandler<ArchiveVacancyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ArchiveVacancyCommand(vacancyId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies")
        .RequireAuthorization(policy => policy.RequireRole(
            UserRole.CompanyAdmin.Code,
            UserRole.CompanyEmployee.Code));
    }
}
