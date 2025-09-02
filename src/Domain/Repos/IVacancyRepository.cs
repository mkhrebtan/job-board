using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.IDs;

namespace Domain.Repos;

public interface IVacancyRepository : IRepository<Vacancy, VacancyId>
{
}
