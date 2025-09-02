using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;

namespace Domain.Repos;

public interface ICompanyUserRepository : IRepository<CompanyUser, CompanyUserId>
{
    Task<bool> IsAlreadyAssignedAsync(Guid userId, Guid companyId, CancellationToken ct);

    Task<bool> IsAlreadyAssignedToCompanyAsync(Guid userId, CancellationToken ct);
}
