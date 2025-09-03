using Domain.Abstraction;

namespace Domain.Contexts.JobPostingContext.IDs;

public record VacancyId : Id
{
    public VacancyId()
        : base(Guid.NewGuid())
    {
    }

    public VacancyId(Guid value)
        : base(value)
    {
    }
}