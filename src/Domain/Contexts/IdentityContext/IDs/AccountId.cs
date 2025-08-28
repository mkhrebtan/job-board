using Domain.Abstraction;

namespace Domain.Contexts.IdentityContext.IDs;

public record AccountId : Id<Guid>
{
    public AccountId()
        : base(Guid.NewGuid())
    {
    }
}
