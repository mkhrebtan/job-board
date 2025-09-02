using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;

namespace Domain.Repos;

public interface ICompanyRepository : IRepository<Company, CompanyId>
{
}
