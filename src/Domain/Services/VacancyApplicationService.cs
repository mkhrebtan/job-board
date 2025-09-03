using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.Enums;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Repos.VacancyApplications;
using Domain.Shared.ErrorHandling;

namespace Domain.Services;

public sealed class VacancyApplicationService
{
    private readonly IVacancyApplicationRepository _vacancyApplicationRepository;

    public VacancyApplicationService(IVacancyApplicationRepository vacancyApplicationRepository)
    {
        _vacancyApplicationRepository = vacancyApplicationRepository;
    }

    public async Task<Result<ResumeVacancyApplication>> ApplyToVacancyWithCreatedResumeAsync(User user, Vacancy vacancy, CoverLetter coverLetter, Resume resume)
    {
        var validationResult = await ValidateVacancyApplication(user, vacancy, coverLetter, CancellationToken.None);
        if (validationResult.IsFailure)
        {
            return Result<ResumeVacancyApplication>.Failure(validationResult.Error);
        }

        return ResumeVacancyApplication.Create(user.Id, vacancy.Id, coverLetter, resume.Id);
    }

    public async Task<Result<FileVacancyApplication>> ApplyToVacancyWithFileAsync(User user, Vacancy vacancy, CoverLetter coverLetter, FileUrl fileUrl)
    {
        var validationResult = await ValidateVacancyApplication(user, vacancy, coverLetter, CancellationToken.None);
        if (validationResult.IsFailure)
        {
            return Result<FileVacancyApplication>.Failure(validationResult.Error);
        }

        return FileVacancyApplication.Create(user.Id, vacancy.Id, coverLetter, fileUrl);
    }

    private async Task<Result> ValidateVacancyApplication(User user, Vacancy vacancy, CoverLetter coverLetter, CancellationToken ct)
    {
        if (user.Role != UserRole.JobSeeker)
        {
            return Result.Failure(new Error("VacancyApplicationService.InvalidUserRole", $"Only users with the '{UserRole.JobSeeker.Name}' role can apply to vacancies."));
        }

        if (vacancy.Status != VacancyStatus.Published)
        {
            return Result.Failure(new Error("VacancyApplicationService.InvalidVacancyStatus", $"Applications can only be made to vacancies that are in the '{VacancyStatus.Published.Name}' status."));
        }

        if (coverLetter is null)
        {
            return Result.Failure(new Error("VacancyApplicationService.NullCoverLetter", "Cover letter cannot be null."));
        }

        if (await _vacancyApplicationRepository.HasAlreadyAppliedToVacancyAsync(user.Id.Value, vacancy.Id.Value, ct))
        {
            return Result.Failure(new Error("VacancyApplicationService.AlreadyApplied", "User has already applied to this vacancy."));
        }

        return Result.Success();
    }
}
