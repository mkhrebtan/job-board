using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Users.Login.WithEmail;
using Application.Commands.Users.Login.WithPhoneNumber;

namespace API.Endpoints.Users.Login;

internal sealed class LoginWithPhoneNumber : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/login/phone", async (
            LoginUserWithPhoneNumberCommand command,
            ICommandHandler<LoginUserWithPhoneNumberCommand, LoginUserCommandResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Users");
    }
}
