using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ApplicationContext.Aggregates;

public class FileVacancyApplication : VacancyApplication
{
    private FileVacancyApplication(UserId seekerId, VacancyId vacancyId, CoverLetter coverLetter, FileUrl fileUrl)
        : base(seekerId, vacancyId, coverLetter)
    {
        FileUrl = fileUrl;
    }

    public FileUrl FileUrl { get; private set; }

    public static Result<FileVacancyApplication> Create(UserId seekerId, VacancyId vacancyId, CoverLetter coverLetter, FileUrl fileUrl)
    {
        var validationResult = ValidateApplication(seekerId, vacancyId, coverLetter);
        if (validationResult.IsFailure)
        {
            return Result<FileVacancyApplication>.Failure(validationResult.Error);
        }

        if (fileUrl is null)
        {
            return Result<FileVacancyApplication>.Failure(new Error("FileVacancyApplication.NullFileUrl", "FileUrl cannot be null."));
        }

        var application = new FileVacancyApplication(seekerId, vacancyId, coverLetter, fileUrl);
        return Result<FileVacancyApplication>.Success(application);
    }
}