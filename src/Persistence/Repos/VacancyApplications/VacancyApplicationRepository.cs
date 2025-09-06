using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.ApplicationContext.IDs;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.VacancyApplications;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.VacancyApplications;

internal class VacancyApplicationRepository : GenericRepository<VacancyApplication, VacancyApplicationId>, IVacancyApplicationRepository
{
    public VacancyApplicationRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<VacancyApplication?> GetByIdAsync(VacancyApplicationId id, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<bool> HasAlreadyAppliedToVacancyAsync(UserId userId, VacancyId vacancyId, CancellationToken ct)
    {
        return await _dbSet.AnyAsync(x => x.SeekerId == userId && x.VacancyId == vacancyId, ct);
    }
}
