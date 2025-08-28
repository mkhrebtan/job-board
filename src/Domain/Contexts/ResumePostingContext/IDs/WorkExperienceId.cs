using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.IDs;

public record WorkExperienceId : Id<Guid>
{
    public WorkExperienceId()
        : base(Guid.NewGuid())
    {
    }
}