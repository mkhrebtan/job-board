using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Shared.ValueObjects;

namespace Domain.Repos.Users;

public interface IUserRepository : IRepository<User, UserId>
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct);

    Task<User?> GetByEmailAsync(Email email, CancellationToken ct);

    Task<User?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken ct);

    Task<bool> IsUniqueEmailAsync(Email email, CancellationToken ct);

    Task<bool> IsUniquePhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken ct);
}