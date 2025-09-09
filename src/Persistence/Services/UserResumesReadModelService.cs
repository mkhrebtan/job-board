using Application.Services;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.ReadModels.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

internal class UserResumesReadModelService : IUserResumesReadModelService
{
    private readonly ApplicationDbContext _context;

    public UserResumesReadModelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserResumesReadModel?> GenerateReadModelAsync(ResumeId ResumeId, CancellationToken cancellationToken = default)
    {
        return await _context.Resumes
            .Where(r => r.Id == ResumeId)
            .Select(r => new UserResumesReadModel
            {
                Id = Guid.NewGuid(),
                UserId = r.SeekerId.Value,
                ResumeId = r.Id.Value,
                Title = r.DesiredPosition.Title,
                IsPublished = r.Status.Code == ResumeStatus.Published.Code,
                CreatedAtUtc = r.CreatedAt
            })
            .FirstOrDefaultAsync(CancellationToken.None);
    }
}
