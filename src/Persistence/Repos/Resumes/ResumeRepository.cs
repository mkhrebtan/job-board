using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.Resumes;

internal class ResumeRepository : GenericRepository<Resume, ResumeId>, IResumeRepository
{
    public ResumeRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Resume?> GetByIdAsync(ResumeId id, CancellationToken ct)
    {
        return await _dbSet
            .Include(x => x.Educations)
            .Include(x => x.WorkExperiences)
            .Include(x => x.Languages)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
}
