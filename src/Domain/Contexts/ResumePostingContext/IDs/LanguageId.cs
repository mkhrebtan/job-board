using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.IDs;

public record LanguageId : Id<Guid>
{
    public LanguageId()
        : base(Guid.NewGuid())
    {
    }
}
