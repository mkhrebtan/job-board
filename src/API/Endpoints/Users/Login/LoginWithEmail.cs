using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Users.Login.WithEmail;

namespace API.Endpoints.Users.Login;

internal sealed class LoginWithEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/login/email", async (
            LoginUserWithEmailCommand command,
            ICommandHandler<LoginUserWithEmailCommand, LoginUserCommandResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Users");
    }
}