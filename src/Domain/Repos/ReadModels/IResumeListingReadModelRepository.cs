using Domain.ReadModels.Resumes;

namespace Domain.Repos.ReadModels;

public interface IResumeListingReadModelRepository
{
    IQueryable<ResumeListingReadModel> GetResumesQueryable();

    Task<IEnumerable<ResumeListingReadModel>> MaterializeAsync(IQueryable<ResumeListingReadModel> resumeListingReadModels);

    Task<IEnumerable<ResumeListingReadModel>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(ResumeListingReadModel model);

    Task Remove(Guid resumeId);

    Task Update(Guid resumeId);
}
