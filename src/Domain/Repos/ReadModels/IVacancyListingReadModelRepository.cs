using Domain.ReadModels.Vacancies;

namespace Domain.Repos.ReadModels;

public interface IVacancyListingReadModelRepository
{
    IQueryable<VacancyListingReadModel> GetVacanciesQueryable();

    Task<IEnumerable<VacancyListingReadModel>> MaterializeAsync(IQueryable<VacancyListingReadModel> vacancyListingReadModels, CancellationToken cancellationToken = default);

    Task<IEnumerable<VacancyListingReadModel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<VacancyListingReadModel>> GetAllByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    void Add(VacancyListingReadModel model);

    Task Remove(Guid vacancyId);

    Task Update(Guid vacancyId);
}