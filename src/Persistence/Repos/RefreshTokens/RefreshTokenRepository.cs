using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Repos.RefreshTokens;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.RefreshTokens;

internal class RefreshTokenRepository : GenericRepository<RefreshToken, RefreshTokenId>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Token == token, ct);
    }
}
