using Domain.Abstraction;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Repos.Users;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.Users;

internal class UserRepository : GenericRepository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return _dbSet
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Email.Address == email, ct);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _dbSet
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id.Value == id, ct);
    }

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct)
    {
        return _dbSet
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.PhoneNumber.Number == phoneNumber, ct);
    }

    public Task<bool> IsUniqueEmailAsync(string email, CancellationToken ct)
    {
        return _dbSet.AnyAsync(x => x.Email.Address == email, ct);
    }

    public Task<bool> IsUniquePhoneNumberAsync(string phoneNumber, CancellationToken ct)
    {
        return _dbSet.AnyAsync(x => x.PhoneNumber.Number == phoneNumber, ct);
    }
}
