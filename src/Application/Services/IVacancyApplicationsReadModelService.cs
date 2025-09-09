using Domain.Contexts.ApplicationContext.IDs;
using Domain.ReadModels.Vacancies;

namespace Application.Services;

public interface IVacancyApplicationsReadModelService
{
    Task<VacancyApplicationsReadModel?> GenerateReadModelAsync(VacancyApplicationId vacancyApplicationId, CancellationToken cancellationToken = default);
}
