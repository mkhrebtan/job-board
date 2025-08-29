using Domain.Abstraction;

namespace Domain.Contexts.RecruitmentContext.IDs;

public record CompanyUserId : Id
{
    public CompanyUserId()
        : base(Guid.NewGuid())
    {
    }
}
