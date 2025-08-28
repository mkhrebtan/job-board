using Domain.Abstraction;

namespace Domain.Contexts.ApplicationContext.IDs;

public record VacancyApplicationId : Id<Guid>
{
    public VacancyApplicationId()
        : base(Guid.NewGuid())
    {
    }
}
