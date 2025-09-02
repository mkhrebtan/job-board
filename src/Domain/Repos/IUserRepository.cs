using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;

namespace Domain.Repos;

public interface IUserRepository : IRepository<User, UserId>
{
}