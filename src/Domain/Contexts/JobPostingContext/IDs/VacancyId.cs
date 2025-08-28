using Domain.Abstraction;

namespace Domain.Contexts.JobPostingContext.IDs;

public record VacancyId : Id<Guid>
{
    public VacancyId()
        : base(Guid.NewGuid())
    {
    }
}