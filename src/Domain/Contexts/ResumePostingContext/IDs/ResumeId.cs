using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.IDs;

public record ResumeId : Id
{
    public ResumeId()
        : base(Guid.NewGuid())
    {
    }
}
