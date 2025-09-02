using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.IDs;

namespace Domain.Repos;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task<bool> IsUniqueNameAsync(string normalizedName, CancellationToken ct);
}
