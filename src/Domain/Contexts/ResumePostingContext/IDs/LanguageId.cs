using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.IDs;

public record LanguageId : Id
{
    public LanguageId()
        : base(Guid.NewGuid())
    {
    }
}
