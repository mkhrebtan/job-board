using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;

namespace Domain.Repos.Users;

public interface IUserRepository : IRepository<User, UserId>
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<User?> GetByEmailAsync(string email, CancellationToken ct);

    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct);

    Task<bool> IsUniqueEmailAsync(string email, CancellationToken ct);

    Task<bool> IsUniquePhoneNumber(string phoneNumber, CancellationToken ct);
}