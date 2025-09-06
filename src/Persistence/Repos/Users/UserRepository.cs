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

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Email.Address == email, ct);
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.PhoneNumber.Number == phoneNumber, ct);
    }

    public async Task<bool> IsUniqueEmailAsync(string email, CancellationToken ct)
    {
        return await _dbSet.AnyAsync(x => x.Email.Address == email, ct);
    }

    public async Task<bool> IsUniquePhoneNumberAsync(string phoneNumber, CancellationToken ct)
    {
        return await _dbSet.AnyAsync(x => x.PhoneNumber.Number == phoneNumber, ct);
    }
}
