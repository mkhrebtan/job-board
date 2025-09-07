using Application.Abstraction.Authentication;
using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Repos.Users;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Users.Login.WithEmail;

internal sealed class LoginUserWithEmailCommandHandler : ICommandHandler<LoginUserWithEmailCommand, LoginUserCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserWithEmailCommandHandler(IUserRepository userRepository, ITokenProvider tokenProvider, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginUserCommandResponse>> Handle(LoginUserWithEmailCommand command, CancellationToken cancellationToken = default)
    {
        if (!Helpers.TryCreateVO(() => Email.Create(command.Email), out Email email, out Error error))
        {
            return Result<LoginUserCommandResponse>.Failure(error);
        }

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return Result<LoginUserCommandResponse>.Failure(Error.NotFound("User.InvalidCredentials", "Invalid login credentials."));
        }

        if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, command.Password))
        {
            return Result<LoginUserCommandResponse>.Failure(Error.Problem("User.InvalidCredentials", "Invalid login credentials."));
        }

        string accessToken = _tokenProvider.GenerateAccessToken(user);
        RefreshToken refreshToken = _tokenProvider.GenerateRefreshToken(user);

        return Result<LoginUserCommandResponse>.Success(new LoginUserCommandResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken.Token));
    }
}