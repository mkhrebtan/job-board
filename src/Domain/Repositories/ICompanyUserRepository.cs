using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;

namespace Domain.Repos;

public interface ICompanyUserRepository : IRepository<CompanyUser, CompanyUserId>
{
    Task<bool> IsAlreadyAssigned(Guid userId, Guid companyId, CancellationToken ct);

    Task<bool> IsAlreadyAssignedToCompany(Guid userId, CancellationToken ct);
}
