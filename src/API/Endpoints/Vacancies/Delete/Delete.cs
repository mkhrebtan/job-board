using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.Delete;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Vacancies.Delete;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("vacancies/{vacancyId:guid}", async (
            Guid vacancyId,
            ICommandHandler<DeleteVacancyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteVacancyCommand(vacancyId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies")
        .RequireAuthorization(policy => policy.RequireRole(
            UserRole.CompanyAdmin.Code,
            UserRole.CompanyEmployee.Code));
    }
}
