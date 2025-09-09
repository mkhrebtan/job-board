using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.ReadModels.Vacancies;

namespace Application.Services;

public interface IRegisteredVacanciesReadModelService
{
    Task<RegisteredVacanciesReadModel?> GenerateReadModelAsync(VacancyId VacancyId, UserId userId, CancellationToken cancellationToken = default);
}
