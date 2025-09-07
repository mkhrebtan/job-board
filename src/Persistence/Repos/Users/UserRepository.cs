using Domain.Abstraction;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Repos.Users;
using Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.Users;

internal class UserRepository : GenericRepository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<User?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken ct)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber, ct);
    }

    public async Task<bool> IsUniqueEmailAsync(Email email, CancellationToken ct)
    {
        return !await _dbSet.AnyAsync(x => x.Email == email, ct);
    }

    public async Task<bool> IsUniquePhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken ct)
    {
        return !await _dbSet.AnyAsync(x => x.PhoneNumber == phoneNumber, ct);
    }
}
