using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.IDs;

namespace Domain.Repos.Categories;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    Task<bool> IsUniqueNameAsync(string normalizedName, CancellationToken ct);

    Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct);

    Task<bool> HasAssignedVacancies(Category category, CancellationToken ct);

    Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct);
}
