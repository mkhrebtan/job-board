using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Companies.UpdateDescription;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Companies.UpdateDescription;

internal sealed class UpdateDescription : IEndpoint
{
    internal sealed record UpdateCompanyDescriptionRequest(string Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/companies/{id:guid}/description", async (
            Guid id,
            UpdateCompanyDescriptionRequest request,
            ICommandHandler<UpdateCompanyDescriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCompanyDescriptionCommand(id, request.Description);
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
