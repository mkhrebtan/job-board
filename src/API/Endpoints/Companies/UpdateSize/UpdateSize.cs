using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Companies.UpdateSize;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Companies.UpdateSize;

internal sealed class UpdateSize : IEndpoint
{
    internal sealed record UpdateCompanySizeRequest(int Size);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/companies/{id:guid}/size", async (
            Guid id,
            int size,
            ICommandHandler<UpdateCompanySizeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCompanySizeCommand(id, size);
            var result = await handler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.GetProblem();
            }

            return Results.NoContent();
        })
        .WithTags("Companies")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.CompanyAdmin.ToString()));
    }
}
