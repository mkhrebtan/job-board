using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.IDs;

namespace Domain.Repos.Vacancies;

public interface IVacancyRepository : IRepository<Vacancy, VacancyId>
{
    Task<Vacancy?> GetByIdAsync(Guid id, CancellationToken ct);
}
