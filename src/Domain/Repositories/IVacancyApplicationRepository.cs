using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.ApplicationContext.IDs;

namespace Domain.Repos;

public interface IVacancyApplicationRepository : IRepository<VacancyApplication, VacancyApplicationId>
{
    Task<bool> HasAlreadyAppliedToVacancyAsync(Guid userId, Guid vacancyId, CancellationToken ct);
}