using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;

namespace Domain.Repos;

public interface IRefreshTokenRepository : IRepository<RefreshToken, RefreshTokenId>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
}
