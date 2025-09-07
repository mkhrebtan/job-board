using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Companies.UpdateName;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Companies.UpdateName;

internal sealed class UpdateName : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/companies/{id:guid}/name", async (
            Guid id,
            string name,
            ICommandHandler<UpdateCompanyNameCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCompanyNameCommand(id, name);
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
