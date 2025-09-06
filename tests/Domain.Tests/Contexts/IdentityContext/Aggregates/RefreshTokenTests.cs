using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;

namespace Domain.Tests.Contexts.IdentityContext.Aggregates;

public class RefreshTokenTests
{
    [Fact]
    public void Create_WithValidInputs_ShouldReturnSuccess()
    {
        var userId = new UserId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var result = RefreshToken.Create(userId, token, expiresAt);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(token, result.Value.Token);
        Assert.Equal(expiresAt, result.Value.ExpiresAt);
        Assert.False(result.Value.IsRevoked);
    }

    [Fact]
    public void Create_WithNullUserId_ShouldReturnFailure()
    {
        UserId? userId = null;
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var result = RefreshToken.Create(userId!, token, expiresAt);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidToken_ShouldReturnFailure(string invalidToken)
    {
        var userId = new UserId();
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var result = RefreshToken.Create(userId, invalidToken, expiresAt);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithPastExpiryDate_ShouldReturnFailure()
    {
        var userId = new UserId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(-1);

        var result = RefreshToken.Create(userId, token, expiresAt);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithCurrentTime_ShouldReturnFailure()
    {
        var userId = new UserId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow;

        var result = RefreshToken.Create(userId, token, expiresAt);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Revoke_WhenNotRevoked_ShouldReturnSuccessAndMarkAsRevoked()
    {
        var userId = new UserId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var refreshToken = RefreshToken.Create(userId, token, expiresAt).Value;

        var result = refreshToken.Revoke();

        Assert.True(result.IsSuccess);
        Assert.True(refreshToken.IsRevoked);
    }

    [Fact]
    public void Revoke_WhenAlreadyRevoked_ShouldReturnFailure()
    {
        var userId = new UserId();
        var token = "valid-refresh-token";
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var refreshToken = RefreshToken.Create(userId, token, expiresAt).Value;
        refreshToken.Revoke();

        var result = refreshToken.Revoke();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }
}
