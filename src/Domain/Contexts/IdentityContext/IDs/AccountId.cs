using Domain.Abstraction;

namespace Domain.Contexts.IdentityContext.IDs;

public record AccountId : Id
{
    public AccountId()
        : base(Guid.NewGuid())
    {
    }
}
