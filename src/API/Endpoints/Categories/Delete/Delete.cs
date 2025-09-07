using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Categories.Delete;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Categories.Delete;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/categories/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteCategoryCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteCategoryCommand(id);
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
