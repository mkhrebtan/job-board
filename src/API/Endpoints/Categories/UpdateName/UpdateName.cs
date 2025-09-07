using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Categories.UpdateName;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Categories.UpdateName;

internal sealed class UpdateName : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/categories/{id:guid}/name", async (
            Guid id,
            string name,
            ICommandHandler<UpdateCategoryNameCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCategoryNameCommand(id, name);
            var result = await handler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.GetProblem();
            }

            return Results.NoContent();
        })
        .WithTags("Categories")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.Admin.ToString()));
    }
}
