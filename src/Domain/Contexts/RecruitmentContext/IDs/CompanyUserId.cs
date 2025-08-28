using Domain.Abstraction;

namespace Domain.Contexts.RecruitmentContext.IDs;

public record CompanyUserId : Id<Guid>
{
    public CompanyUserId()
        : base(Guid.NewGuid())
    {
    }
}
