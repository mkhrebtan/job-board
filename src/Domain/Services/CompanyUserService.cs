using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Repos.CompanyUsers;
using Domain.Shared.ErrorHandling;

namespace Domain.Services;

public sealed class CompanyUserService
{
    private readonly ICompanyUserRepository _companyUserRepository;

    public CompanyUserService(ICompanyUserRepository companyUserRepository)
    {
        _companyUserRepository = companyUserRepository;
    }

    public async Task<Result<CompanyUser>> AssignEmployerToCompanyAsync(User user, Company company, CancellationToken ct)
    {
        if (user.Role != UserRole.Employer)
        {
            return Result<CompanyUser>.Failure(new Error("CompanyUserService.InvalidUserRole", $"Only users with the '{UserRole.Employer.Name}' role can be assigned to a company."));
        }

        if (await _companyUserRepository.IsAlreadyAssignedToCompanyAsync(user.Id.Value, ct))
        {
            return Result<CompanyUser>.Failure(new Error("CompanyUserService.UserAlreadyAssigned", "This user is already assigned to a company."));
        }

        if (await _companyUserRepository.IsAlreadyAssignedAsync(user.Id.Value, company.Id.Value, ct))
        {
            return Result<CompanyUser>.Failure(new Error("CompanyUserService.UserAlreadyAssignedToCompany", "This user is already assigned to this company."));
        }

        return CompanyUser.Create(user.Id, company.Id);
    }
}
