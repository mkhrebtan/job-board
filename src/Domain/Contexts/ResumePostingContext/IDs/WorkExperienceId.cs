using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.IDs;

public record WorkExperienceId : Id
{
    public WorkExperienceId()
        : base(Guid.NewGuid())
    {
    }

    public WorkExperienceId(Guid value)
        : base(value)
    {
    }
}