using Domain.ReadModels.Vacancies;

namespace Domain.Repos.ReadModels;

public interface IVacancyApplicationsReadModelRepository
{
    IQueryable<VacancyApplicationsReadModel> GetVacancyApplicationsReadModelsQueryable(Guid vacancyId);

    Task<IEnumerable<VacancyApplicationsReadModel>> GetVacancyApplicationsAsync(Guid vacancyId, CancellationToken cancellationToken = default);

    void Add(VacancyApplicationsReadModel model);

    Task Remove(Guid vacancyId);

    Task RemoveByResume(Guid resumeId);

    Task UpdateByResume(Guid resumeId);
}