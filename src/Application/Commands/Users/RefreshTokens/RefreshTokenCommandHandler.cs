using Application.Abstraction.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Repos.RefreshTokens;
using Domain.Repos.Users;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Users.RefreshTokens;

internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, ITokenProvider tokenProvider, IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RefreshTokenCommandResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(command.Token, cancellationToken);
        if (existingToken is null || existingToken.ExpiresAt < DateTime.UtcNow || existingToken.IsRevoked)
        {
            return Result<RefreshTokenCommandResponse>.Failure(Error.Problem("RefreshToken.Invalid", "The provided refresh token is invalid or has expired."));
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
        if (user is null)
        {
            return Result<RefreshTokenCommandResponse>.Failure(Error.Problem("User.NotFound", "User not found."));
        }

        existingToken.Revoke();
        string accessToken = _tokenProvider.GenerateAccessToken(user);
        RefreshToken refreshToken = _tokenProvider.GenerateRefreshToken(user);

        _refreshTokenRepository.Update(existingToken);
        _refreshTokenRepository.Add(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RefreshTokenCommandResponse>.Success(new RefreshTokenCommandResponse(accessToken, refreshToken.Token));
    }
}
