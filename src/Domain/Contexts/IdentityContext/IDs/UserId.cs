using Domain.Abstraction;

namespace Domain.Contexts.IdentityContext.IDs;

public record UserId : Id<Guid>
{
    public UserId()
        : base(Guid.NewGuid())
    {
    }
}
