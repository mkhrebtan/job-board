using Domain.Abstraction;

namespace Domain.Contexts.JobPostingContext.IDs;

public record CategoryId : Id
{
    public CategoryId()
        : base(Guid.NewGuid())
    {
    }

    public CategoryId(Guid value)
        : base(value)
    {
    }
}