using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Categories.Create;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Categories.Create;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/categories", async (
            string name,
            ICommandHandler<CreateCategoryCommand, CreateCategoryResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateCategoryCommand(name);
            var result = await handler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.GetProblem();
            }

            return Results.Created($"/categories/{result.Value.Id}", result.Value);
        })
        .WithTags("Categories")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.Admin.ToString()));
    }
}
