using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.IDs;

namespace Domain.Repos.Resumes;

public interface IResumeRepository : IRepository<Resume, ResumeId>
{
    Task<Resume?> GetByIdAsync(ResumeId id, CancellationToken ct);
}
