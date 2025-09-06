using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Companies.UpdateLogo;

namespace API.Endpoints.Companies.UpdateLogo;

internal sealed class UpdateLogo : IEndpoint
{
    internal sealed record UpdateCompanyLogoRequest(string LogoUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/companies/{id:guid}/logo", async (
            Guid id,
            UpdateCompanyLogoRequest request,
            ICommandHandler<UpdateCompanyLogoCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateCompanyLogoCommand(id, request.LogoUrl);
            var result = await handler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.GetProblem();
            }

            return Results.NoContent();
        })
        .WithTags("Companies");
    }
}
