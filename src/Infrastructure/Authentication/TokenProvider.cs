using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Application.Abstraction.Authentication;
using Domain.Contexts.IdentityContext.Aggregates;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

internal sealed class TokenProvider : ITokenProvider
{
    private readonly IOptions<JwtSettings> _jwtSettings;

    public TokenProvider(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Address),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Value.Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtSettings.Value.Audience),
        };

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.Value.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Value.Issuer,
            audience: _jwtSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.AccessTokenExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var token = Convert.ToBase64String(randomBytes);
        var refreshTokenResult = RefreshToken.Create(user.Id, token, DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenExpirationDays));
        if (refreshTokenResult.IsFailure)
        {
            throw new ApplicationException("Failed to create refresh token: " + refreshTokenResult.Error);
        }

        return refreshTokenResult.Value;
    }
}
