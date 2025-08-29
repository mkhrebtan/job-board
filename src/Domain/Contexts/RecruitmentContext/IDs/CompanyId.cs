using Domain.Abstraction;

namespace Domain.Contexts.RecruitmentContext.IDs;

public record CompanyId : Id
{
    public CompanyId()
        : base(Guid.NewGuid())
    {
    }
}
