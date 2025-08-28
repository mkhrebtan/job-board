using Domain.Abstraction;
using Domain.Contexts.ApplicationContext.IDs;
using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ApplicationContext.Aggregates;

public abstract class VacancyApplication : Entity<VacancyApplicationId>
{
    protected VacancyApplication(UserId seekerId, VacancyId vacancyId, CoverLetter coverLetter)
        : base(new VacancyApplicationId())
    {
        SeekerId = seekerId;
        VacancyId = vacancyId;
        CoverLetter = coverLetter;
    }

    public UserId SeekerId { get; private set; }

    public VacancyId VacancyId { get; private set; }

    public CoverLetter CoverLetter { get; private set; }

    protected static Result ValidateApplication(UserId seekerId, VacancyId vacancyId, CoverLetter coverLetter)
    {
        if (seekerId == null || seekerId.Value == Guid.Empty)
        {
            return Result.Failure(new Error("VacancyApplication.InvalidSeekerId", "SeekerId cannot be null or empty."));
        }

        if (vacancyId == null || vacancyId.Value == Guid.Empty)
        {
            return Result.Failure(new Error("VacancyApplication.InvalidVacancyId", "VacancyId cannot be null or empty."));
        }

        if (coverLetter == null)
        {
            return Result.Failure(new Error("VacancyApplication.NullCoverLetter", "Cover letter cannot be null."));
        }

        return Result.Success();
    }
}