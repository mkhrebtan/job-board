using Domain.Contexts.ResumePostingContext.IDs;
using Domain.ReadModels.Resumes;

namespace Application.Services;

public interface IResumeListingReadModelService
{
    Task<ResumeListingReadModel?> GenerateReadModelAsync(ResumeId ResumeId, CancellationToken cancellationToken = default);
}
