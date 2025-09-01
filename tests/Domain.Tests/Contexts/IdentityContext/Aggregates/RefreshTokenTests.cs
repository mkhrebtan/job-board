using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;

namespace Domain.Tests.Contexts.IdentityContext.Aggregates;

public class RefreshTokenTests
{
    [Fact]
    public void Create_WithValidInputs_ShouldReturnSuccess()
    {
        var accountId = new AccountId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var result = RefreshToken.Create(accountId, token, expiresAt);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(accountId, result.Value.AccountId);
        Assert.Equal(token, result.Value.Token);
        Assert.Equal(expiresAt, result.Value.ExpiresAt);
        Assert.False(result.Value.IsRevoked);
    }

    [Fact]
    public void Create_WithNullAccountId_ShouldReturnFailure()
    {
        AccountId? accountId = null;
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var result = RefreshToken.Create(accountId!, token, expiresAt);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidToken_ShouldReturnFailure(string invalidToken)
    {
        var accountId = new AccountId();
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var result = RefreshToken.Create(accountId, invalidToken, expiresAt);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithPastExpiryDate_ShouldReturnFailure()
    {
        var accountId = new AccountId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(-1);

        var result = RefreshToken.Create(accountId, token, expiresAt);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithCurrentTime_ShouldReturnFailure()
    {
        var accountId = new AccountId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow;

        var result = RefreshToken.Create(accountId, token, expiresAt);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Revoke_WhenNotRevoked_ShouldReturnSuccessAndMarkAsRevoked()
    {
        var accountId = new AccountId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var refreshToken = RefreshToken.Create(accountId, token, expiresAt).Value;

        var result = refreshToken.Revoke();

        Assert.True(result.IsSuccess);
        Assert.True(refreshToken.IsRevoked);
    }

    [Fact]
    public void Revoke_WhenAlreadyRevoked_ShouldReturnFailure()
    {
        var accountId = new AccountId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var refreshToken = RefreshToken.Create(accountId, token, expiresAt).Value;
        refreshToken.Revoke();

        var result = refreshToken.Revoke();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }
}
