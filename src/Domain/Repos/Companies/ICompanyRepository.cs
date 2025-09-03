using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;

namespace Domain.Repos.Companies;

public interface ICompanyRepository : IRepository<Company, CompanyId>
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken ct);
}
