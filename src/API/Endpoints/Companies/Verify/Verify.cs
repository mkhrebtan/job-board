using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Companies.Verify;

namespace API.Endpoints.Companies.Verify;

public class Verify : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/companies/{id:guid}/verify", async (
            Guid id,
            ICommandHandler<VerifyCompanyCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new VerifyCompanyCommand(id);
            var result = await handler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.GetProblem();
            }

            return Results.Ok();
        })
        .WithTags("Companies");
    }
}
