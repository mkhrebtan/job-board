using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.Vacancies;

internal class VacancyRepository : GenericRepository<Vacancy, VacancyId>, IVacancyRepository
{
    public VacancyRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public Task<Vacancy?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _dbSet.FirstOrDefaultAsync(x => x.Id.Value == id, ct);
    }
}
