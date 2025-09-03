using Domain.Abstraction;

namespace Domain.Contexts.IdentityContext.IDs;

public record RefreshTokenId : Id
{
    public RefreshTokenId()
        : base(Guid.NewGuid())
    {
    }

    public RefreshTokenId(Guid id)
        : base(id)
    {
    }
}
