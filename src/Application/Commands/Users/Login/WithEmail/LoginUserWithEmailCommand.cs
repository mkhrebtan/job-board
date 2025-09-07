using Application.Abstractions.Messaging;

namespace Application.Commands.Users.Login.WithEmail;

public record LoginUserWithEmailCommand(string Email, string Password) : ICommand<LoginUserCommandResponse>;

public record LoginUserCommandResponse(
    string AccessToken,
    string RefreshToken);
