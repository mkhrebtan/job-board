using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Companies.UpdateWebsite;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Companies.UpdateWebsite;

internal sealed class UpdateWebsite : IEndpoint
{
    internal sealed record UpdateCompanyWebsiteRequest(string WebsiteUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/companies/{id:guid}/website", async (
            Guid id,
            UpdateCompanyWebsiteRequest request,
            ICommandHandler<UpdateCompanyWebsiteCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCompanyWebsiteCommand(id, request.WebsiteUrl);
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
