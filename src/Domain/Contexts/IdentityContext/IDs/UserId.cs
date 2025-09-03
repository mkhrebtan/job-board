using Domain.Abstraction;

namespace Domain.Contexts.IdentityContext.IDs;

public record UserId : Id
{
    public UserId()
        : base(Guid.NewGuid())
    {
    }

    public UserId(Guid id)
        : base(id)
    {
    }
}
