using Domain.Abstraction;

namespace Domain.Contexts.JobPostingContext.IDs;

public record CategoryId : Id<Guid>
{
    public CategoryId()
        : base(Guid.NewGuid())
    {
    }
}