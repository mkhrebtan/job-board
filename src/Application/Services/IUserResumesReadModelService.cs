using Domain.Contexts.ResumePostingContext.IDs;
using Domain.ReadModels.Resumes;

namespace Application.Services;

public interface IUserResumesReadModelService
{
    Task<UserResumesReadModel?> GenerateReadModelAsync(ResumeId ResumeId, CancellationToken cancellationToken = default);
}