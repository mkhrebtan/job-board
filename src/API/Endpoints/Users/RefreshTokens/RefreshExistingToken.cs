using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Users.RefreshTokens;

namespace API.Endpoints.Users.RefreshTokens;

internal sealed class RefreshExistingToken : IEndpoint
{
    internal sealed record RefreshTokenRequest(string Token);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh-token", async (
            RefreshTokenRequest request,
            ICommandHandler<RefreshTokenCommand, RefreshTokenCommandResponse> commandHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await commandHandler.Handle(new RefreshTokenCommand(request.Token), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Users");
    }
}