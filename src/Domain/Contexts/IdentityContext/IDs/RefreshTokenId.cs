using Domain.Abstraction;

namespace Domain.Contexts.IdentityContext.IDs;

public record RefreshTokenId : Id<Guid>
{
    public RefreshTokenId()
        : base(Guid.NewGuid())
    {
    }
}
