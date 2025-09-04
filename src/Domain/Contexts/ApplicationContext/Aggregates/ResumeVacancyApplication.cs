using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ApplicationContext.Aggregates;

public class ResumeVacancyApplication : VacancyApplication
{
    private ResumeVacancyApplication(UserId seekerId, VacancyId vacancyId, CoverLetter coverLetter, ResumeId resumeId)
        : base(seekerId, vacancyId, coverLetter)
    {
        ResumeId = resumeId;
    }

    public ResumeId ResumeId { get; private set; }

    internal static Result<ResumeVacancyApplication> Create(UserId seekerId, VacancyId vacancyId, CoverLetter coverLetter, ResumeId resumeId)
    {
        var validationResult = ValidateApplication(seekerId, vacancyId, coverLetter);
        if (validationResult.IsFailure)
        {
            return Result<ResumeVacancyApplication>.Failure(validationResult.Error);
        }

        if (resumeId is null || resumeId.Value == Guid.Empty)
        {
            return Result<ResumeVacancyApplication>.Failure(Error.Problem("ResumeVacancyApplication.InvalidResumeId", "ResumeId cannot be null or empty."));
        }

        var application = new ResumeVacancyApplication(seekerId, vacancyId, coverLetter, resumeId);
        return Result<ResumeVacancyApplication>.Success(application);
    }
}
