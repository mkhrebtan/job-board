using Application.Abstractions.Messaging;

namespace Application.Commands.Users.RefreshTokens;

public record RefreshTokenCommand(string Token) : ICommand<RefreshTokenCommandResponse>;

public record RefreshTokenCommandResponse(string AccessToken, string RefreshToken);