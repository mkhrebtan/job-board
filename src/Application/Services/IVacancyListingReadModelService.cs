using Domain.Contexts.JobPostingContext.IDs;
using Domain.ReadModels.Vacancies;

namespace Application.Services;

public interface IVacancyListingReadModelService
{
    Task<VacancyListingReadModel?> GenerateReadModelAsync(VacancyId VacancyId, CancellationToken cancellationToken = default);
}
