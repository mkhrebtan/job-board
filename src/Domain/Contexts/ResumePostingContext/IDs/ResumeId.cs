using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.IDs;

public record ResumeId : Id<Guid>
{
    public ResumeId()
        : base(Guid.NewGuid())
    {
    }
}
