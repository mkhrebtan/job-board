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

    public async Task<CompanyUser?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id.Value == id, ct);
    }

    public async Task<bool> IsAlreadyAssignedAsync(Guid userId, Guid companyId, CancellationToken ct)
    {
        return await _dbSet.AnyAsync(x => x.RecruiterId.Value == userId && x.CompanyId.Value == companyId, ct);
    }

    public Task<bool> IsAlreadyAssignedToCompanyAsync(Guid userId, CancellationToken ct)
    {
        return _dbSet.AnyAsync(x => x.RecruiterId.Value == userId, ct);
    }
}
