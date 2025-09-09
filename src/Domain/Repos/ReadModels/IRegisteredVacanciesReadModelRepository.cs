using Domain.ReadModels.Vacancies;

namespace Domain.Repos.ReadModels;

public interface IRegisteredVacanciesReadModelRepository
{
    IQueryable<RegisteredVacanciesReadModel> GetRegisteredVacanciesQueryable();

    Task<IEnumerable<RegisteredVacanciesReadModel>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(RegisteredVacanciesReadModel model);

    Task Remove(Guid vacancyId);

    Task Update(Guid vacancyId);
}
