using Domain.Abstraction;

namespace Domain.Contexts.ApplicationContext.IDs;

public record VacancyApplicationId : Id
{
    public VacancyApplicationId()
        : base(Guid.NewGuid())
    {
    }
}
