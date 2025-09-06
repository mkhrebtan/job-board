using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.CompanyUsers;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.CompanyUsers;

internal class CompanyUserRepository : GenericRepository<CompanyUser, CompanyUserId>, ICompanyUserRepository
{
    public CompanyUserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<CompanyUser?> GetByIdAsync(CompanyUserId id, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<CompanyId?> GetCompanyIdByUserId(UserId userId, CancellationToken ct)
    {
        return await _dbSet
            .Where(x => x.RecruiterId == userId)
            .Select(x => x.CompanyId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> IsAlreadyAssignedAsync(UserId userId, CompanyId companyId, CancellationToken ct)
    {
        return await _dbSet.AnyAsync(x => x.RecruiterId == userId && x.CompanyId == companyId, ct);
    }

    public Task<bool> IsAlreadyAssignedToCompanyAsync(UserId userId, CancellationToken ct)
    {
        return _dbSet.AnyAsync(x => x.RecruiterId == userId, ct);
    }
}
