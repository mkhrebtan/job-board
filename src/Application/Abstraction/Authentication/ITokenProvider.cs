using Domain.Contexts.IdentityContext.Aggregates;

namespace Application.Abstraction.Authentication;

public interface ITokenProvider
{
    string GenerateAccessToken(User user);

    RefreshToken GenerateRefreshToken(User user);
}
