using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;

namespace Domain.Repos.CompanyUsers;

public interface ICompanyUserRepository : IRepository<CompanyUser, CompanyUserId>
{
    Task<bool> IsAlreadyAssignedAsync(UserId userId, CompanyId companyId, CancellationToken ct);

    Task<bool> IsAlreadyAssignedToCompanyAsync(UserId userId, CancellationToken ct);

    Task<CompanyUser?> GetByIdAsync(CompanyUserId id, CancellationToken ct);

    Task<CompanyId?> GetCompanyIdByUserId(UserId userId, CancellationToken ct);
}
