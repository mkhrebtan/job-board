using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.ReadModels.Resumes;
using Domain.Repos.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.ReadModels;

internal class UserResumesReadModelRepository : IUserResumesReadModelRepository
{
    private readonly ApplicationDbContext _context;

    public UserResumesReadModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResumesReadModel>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserResumes
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
    }

    public void Add(UserResumesReadModel model)
    {
        _context.UserResumes.Add(model);
    }

    public async Task Remove(Guid resumeId)
    {
        var modelToRemove = await _context.UserResumes.FirstOrDefaultAsync(ur => ur.ResumeId == resumeId);
        if (modelToRemove != null)
        {
            _context.UserResumes.Remove(modelToRemove);
        }
    }

    public async Task Update(Guid resumeId)
    {
        var modelToUpdate = await _context.UserResumes.FirstOrDefaultAsync(ur => ur.ResumeId == resumeId);
        if (modelToUpdate != null)
        {
            var resumeData = await _context.Resumes
                .Where(r => r.Id == new ResumeId(resumeId))
                .Select(r => new
                {
                    r.DesiredPosition.Title,
                    IsPublished = r.Status.Code == ResumeStatus.Published.Code
                })
                .FirstOrDefaultAsync();

            if (resumeData != null)
            {
                modelToUpdate.Title = resumeData.Title;
                modelToUpdate.IsPublished = resumeData.IsPublished;

                _context.UserResumes.Update(modelToUpdate);
            }
        }
    }

    public IQueryable<UserResumesReadModel> GetUserResumesQueryable(Guid userId)
    {
        return _context.UserResumes.Where(x => x.UserId == userId).AsQueryable();
    }

    public async Task<IEnumerable<UserResumesReadModel>> MaterializeAsync(IQueryable<UserResumesReadModel> userResumesReadModels, CancellationToken cancellationToken = default)
    {
        return await userResumesReadModels.ToListAsync(cancellationToken);
    }
}