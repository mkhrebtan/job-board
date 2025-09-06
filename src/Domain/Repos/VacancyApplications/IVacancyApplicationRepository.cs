using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.ApplicationContext.IDs;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.IDs;

namespace Domain.Repos.VacancyApplications;

public interface IVacancyApplicationRepository : IRepository<VacancyApplication, VacancyApplicationId>
{
    Task<bool> HasAlreadyAppliedToVacancyAsync(UserId userId, VacancyId vacancyId, CancellationToken ct);

    Task<VacancyApplication?> GetByIdAsync(VacancyApplicationId id, CancellationToken ct);
}