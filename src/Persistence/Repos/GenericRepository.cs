using Domain.Abstraction;
using Domain.Repos;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos;

public class GenericRepository<TRoot, TId> : IRepository<TRoot, TId>
    where TRoot : AggregateRoot<TId>
    where TId : Id
{
    protected readonly ApplicationDbContext _context;

    protected readonly DbSet<TRoot> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TRoot>() ?? throw new InvalidOperationException($"Entity type {typeof(TRoot).Name} is not registered in the DbContext.");
    }

    public void Add(TRoot root)
    {
        _dbSet.Add(root);
    }

    public void Remove(TRoot root)
    {
        _dbSet.Remove(root);
    }

    public void Update(TRoot root)
    {
        _dbSet.Update(root);
    }
}
