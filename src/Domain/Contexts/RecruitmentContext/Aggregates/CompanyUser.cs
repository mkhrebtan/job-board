using Domain.Abstraction;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.RecruitmentContext.Aggregates;

public class CompanyUser : AggregateRoot<CompanyUserId>
{
    private CompanyUser()
        : base(new CompanyUserId())
    {
    }

    private CompanyUser(UserId recruiterId, CompanyId companyId)
        : base(new CompanyUserId())
    {
        RecruiterId = recruiterId;
        CompanyId = companyId;
    }

    public UserId RecruiterId { get; private set; }

    public CompanyId CompanyId { get; private set; }

    internal static Result<CompanyUser> Create(UserId recruiterId, CompanyId companyId)
    {
        if (recruiterId == null)
        {
            return Result<CompanyUser>.Failure(new Error("CompanyUser.NullRecruiterId", "Recruiter ID cannot be null."));
        }

        if (companyId == null)
        {
            return Result<CompanyUser>.Failure(new Error("CompanyUser.NullCompanyId", "Company ID cannot be null."));
        }

        var companyUser = new CompanyUser(recruiterId, companyId);
        return Result<CompanyUser>.Success(companyUser);
    }
}
