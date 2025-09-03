using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.ApplicationContext.IDs;
using Domain.Repos.VacancyApplications;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.VacancyApplications;

internal class VacancyApplicationRepository : GenericRepository<VacancyApplication, VacancyApplicationId>, IVacancyApplicationRepository
{
    public VacancyApplicationRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public Task<VacancyApplication?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _dbSet.FirstOrDefaultAsync(x => x.Id.Value == id, ct);
    }

    public Task<bool> HasAlreadyAppliedToVacancyAsync(Guid userId, Guid vacancyId, CancellationToken ct)
    {
        return _dbSet.AnyAsync(x => x.SeekerId.Value == userId && x.VacancyId.Value == vacancyId, ct);
    }
}
