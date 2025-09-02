using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.IDs;

namespace Domain.Repos;

public interface IResumeRepository : IRepository<Resume, ResumeId>
{
}
