using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.Categories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.Categories;

internal class CategoryRepository : GenericRepository<Category, CategoryId>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id.Value == id, ct);
    }

    public async Task<bool> IsUniqueNameAsync(string normalizedName, CancellationToken ct)
    {
        return await _dbSet.AnyAsync(x => x.NormalizedName == normalizedName, ct);
    }
}
