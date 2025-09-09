using Domain.Contexts.JobPostingContext.IDs;
using Domain.ReadModels.Vacancies;

namespace Application.Services;

public interface ICompanyVacanciesReadModelService
{
    Task<CompanyVacanciesReadModel?> GenerateReadModelAsync(VacancyId VacancyId, CancellationToken cancellationToken = default);
}
