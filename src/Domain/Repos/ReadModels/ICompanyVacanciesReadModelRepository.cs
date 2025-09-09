using Domain.ReadModels.Vacancies;

namespace Domain.Repos.ReadModels;

public interface ICompanyVacanciesReadModelRepository
{
    IQueryable<CompanyVacanciesReadModel> GetCompanyVacanciesQueryable(Guid companyId);

    Task<IEnumerable<CompanyVacanciesReadModel>> GetAllByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);

    Task<IEnumerable<CompanyVacanciesReadModel>> GetPublishedByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);

    void Add(CompanyVacanciesReadModel model);

    Task Remove(Guid vacancyId);

    Task Update(Guid vacancyId);
}
