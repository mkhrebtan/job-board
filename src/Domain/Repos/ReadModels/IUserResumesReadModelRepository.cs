using Domain.ReadModels.Resumes;

namespace Domain.Repos.ReadModels;

public interface IUserResumesReadModelRepository
{
    Task<IEnumerable<UserResumesReadModel>> GetByUserIdAsync(Guid userId);

    void Add(UserResumesReadModel model);

    Task Remove(Guid resumeId);

    Task Update(Guid resumeId);
}