using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Companies.Create;

namespace API.Endpoints.Companies.Create;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/companies", async (
            ICommandHandler<CreateCompanyCommand, CreateCompanyCommandResponse> handler,
            CreateCompanyCommand command,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.GetProblem();
            }

            return Results.Created($"/companies/{result.Value.Id}", result.Value);
        })
        .WithTags("Companies");
    }
}
