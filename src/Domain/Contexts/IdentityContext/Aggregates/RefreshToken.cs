using Domain.Abstraction;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.IdentityContext.Aggregates;

public class RefreshToken : AggregateRoot<RefreshTokenId>
{
    private RefreshToken()
        : base(new RefreshTokenId())
    {
    }

    private RefreshToken(UserId userId, string token, DateTime expiresAt)
        : base(new RefreshTokenId())
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public UserId UserId { get; private set; }

    public string Token { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public bool IsRevoked { get; private set; }

    public static Result<RefreshToken> Create(UserId userId, string token, DateTime expiresAt)
    {
        if (userId == null || userId.Value == Guid.Empty)
        {
            return Result<RefreshToken>.Failure(Error.Problem("RefreshToken.InvalidUserId", "UserId cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return Result<RefreshToken>.Failure(Error.Problem("RefreshToken.InvalidToken", "Token cannot be null or empty."));
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            return Result<RefreshToken>.Failure(Error.Problem("RefreshToken.InvalidExpiry", "Expiry date must be in the future."));
        }

        var refreshToken = new RefreshToken(userId, token, expiresAt);
        return Result<RefreshToken>.Success(refreshToken);
    }

    public Result Revoke()
    {
        if (IsRevoked)
        {
            return Result.Failure(Error.Conflict("RefreshToken.AlreadyRevoked", "Refresh token is already revoked."));
        }

        IsRevoked = true;
        return Result.Success();
    }
}
