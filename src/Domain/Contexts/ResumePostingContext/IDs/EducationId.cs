using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.IDs;

public record EducationId : Id<Guid>
{
    public EducationId()
        : base(Guid.NewGuid())
    {
    }
}
