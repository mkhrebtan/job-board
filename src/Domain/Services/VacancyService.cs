using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.CompanyUsers;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Services;

public sealed class VacancyService
{
    private readonly ICompanyUserRepository _companyUserRepository;

    public VacancyService(ICompanyUserRepository companyUserRepository)
    {
        _companyUserRepository = companyUserRepository;
    }

    public async Task<Result<Vacancy>> CreateVacancyInDraftStatusAsync(
        User user,
        VacancyTitle title,
        RichTextContent descripiton,
        Salary salary,
        Location location,
        RecruiterInfo recruiterInfo,
        CancellationToken cancellationToken)
    {
        var companyIdResult = await ValidateVacancyCreationAndGetCompanyIdAsync(user, cancellationToken);
        if (companyIdResult.IsFailure)
        {
            return Result<Vacancy>.Failure(companyIdResult.Error);
        }

        return Vacancy.CreateDraft(title, descripiton, salary, companyIdResult.Value, location, recruiterInfo);
    }

    public async Task<Result<Vacancy>> CreateVacancyInRegisteredStatusAsync(
        User user,
        VacancyTitle title,
        RichTextContent descripiton,
        Salary salary,
        Location location,
        RecruiterInfo recruiterInfo,
        CancellationToken cancellationToken)
    {
        var companyIdResult = await ValidateVacancyCreationAndGetCompanyIdAsync(user, cancellationToken);
        if (companyIdResult.IsFailure)
        {
            return Result<Vacancy>.Failure(companyIdResult.Error);
        }

        return Vacancy.CreateAndRegister(title, descripiton, salary, companyIdResult.Value, location, recruiterInfo);
    }

    private async Task<Result<CompanyId>> ValidateVacancyCreationAndGetCompanyIdAsync(User user, CancellationToken cancellationToken)
    {
        if (user.Role != UserRole.Employer)
        {
            return Result<CompanyId>.Failure(Error.Problem("VacancyService.Unauthorized", "Only users with the Employer role can create vacancies."));
        }

        CompanyId? userCompanyId = await _companyUserRepository.GetCompanyIdByUserId(user.Id, cancellationToken);
        if (userCompanyId is null)
        {
            return Result<CompanyId>.Failure(Error.Problem("VacancyService.NoCompany", "The employer user must be associated with a company to create a vacancy."));
        }

        return Result<CompanyId>.Success(userCompanyId);
    }
}
