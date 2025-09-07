using Application.Abstraction.Authentication;
using Application.Abstractions.Messaging;
using Application.Commands.Users.Login.WithEmail;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Repos.Users;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Users.Login.WithPhoneNumber;

internal sealed class LoginUserWithPhoneNumberCommandHandler : ICommandHandler<LoginUserWithPhoneNumberCommand, LoginUserCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserWithPhoneNumberCommandHandler(IUserRepository userRepository, ITokenProvider tokenProvider, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginUserCommandResponse>> Handle(LoginUserWithPhoneNumberCommand command, CancellationToken cancellationToken = default)
    {
        if (!Helpers.TryCreateVO(() => PhoneNumber.Create(command.PhoneNumber, command.PhoneNumberRegionCode), out PhoneNumber phoneNumber, out Error error))
        {
            return Result<LoginUserCommandResponse>.Failure(error);
        }

        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);
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