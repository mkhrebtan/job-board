using Domain.Abstraction;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.IdentityContext.Aggregates;

public class RefreshToken : AggregateRoot<RefreshTokenId>
{
    private RefreshToken(AccountId accountId, string token, DateTime expiresAt)
        : base(new RefreshTokenId())
    {
        AccountId = accountId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public AccountId AccountId { get; private set; }

    public string Token { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public bool IsRevoked { get; private set; }

    public static Result<RefreshToken> Create(AccountId accountId, string token, DateTime expiresAt)
    {
        if (accountId == null || accountId.Value == Guid.Empty)
        {
            return Result<RefreshToken>.Failure(new Error("RefreshToken.InvalidAccountId", "AccountId cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return Result<RefreshToken>.Failure(new Error("RefreshToken.InvalidToken", "Token cannot be null or empty."));
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            return Result<RefreshToken>.Failure(new Error("RefreshToken.InvalidExpiry", "Expiry date must be in the future."));
        }

        var refreshToken = new RefreshToken(accountId, token, expiresAt);
        return Result<RefreshToken>.Success(refreshToken);
    }

    public Result Revoke()
    {
        if (IsRevoked)
        {
            return Result.Failure(new Error("RefreshToken.AlreadyRevoked", "Refresh token is already revoked."));
        }

        IsRevoked = true;
        return Result.Success();
    }
}
