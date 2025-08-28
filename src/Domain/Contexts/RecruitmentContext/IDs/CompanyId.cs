using Domain.Abstraction;

namespace Domain.Contexts.RecruitmentContext.IDs;

public record CompanyId : Id<Guid>
{
    public CompanyId()
        : base(Guid.NewGuid())
    {
    }
}
