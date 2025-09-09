using Domain.ReadModels.Resumes;

namespace Domain.Repos.ReadModels;

public interface IUserResumesReadModelRepository
{
    IQueryable<UserResumesReadModel> GetUserResumesQueryable(Guid userId);

    Task<IEnumerable<UserResumesReadModel>> MaterializeAsync(IQueryable<UserResumesReadModel> userResumesReadModels, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserResumesReadModel>> GetByUserIdAsync(Guid userId);

    void Add(UserResumesReadModel model);

    Task Remove(Guid resumeId);

    Task Update(Guid resumeId);
}