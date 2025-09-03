using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.IDs;

public record EducationId : Id
{
    public EducationId()
        : base(Guid.NewGuid())
    {
    }

    public EducationId(Guid value)
        : base(value)
    {
    }
}
