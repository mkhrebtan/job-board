using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;

namespace Domain.Repos.CompanyUsers;

public interface ICompanyUserRepository : IRepository<CompanyUser, CompanyUserId>
{
    Task<bool> IsAlreadyAssignedAsync(Guid userId, Guid companyId, CancellationToken ct);

    Task<bool> IsAlreadyAssignedToCompanyAsync(Guid userId, CancellationToken ct);

    Task<CompanyUser?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<Guid?> GetCompanyIdByUserId(Guid userId, CancellationToken ct);
}
