using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.Companies;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.Companies;

internal class CompanyRepository : GenericRepository<Company, CompanyId>, ICompanyRepository
{
    public CompanyRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Company?> GetByIdAsync(CompanyId id, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
