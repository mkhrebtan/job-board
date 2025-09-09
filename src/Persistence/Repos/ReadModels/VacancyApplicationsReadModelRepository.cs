using Domain.Contexts.ResumePostingContext.IDs;
using Domain.ReadModels.Vacancies;
using Domain.Repos.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.ReadModels;

internal class VacancyApplicationsReadModelRepository : IVacancyApplicationsReadModelRepository
{
    private readonly ApplicationDbContext _context;

    public VacancyApplicationsReadModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(VacancyApplicationsReadModel model)
    {
        _context.VacancyApplicationsReadModels.Add(model);
    }

    public async Task<IEnumerable<VacancyApplicationsReadModel>> GetVacancyApplicationsAsync(Guid vacancyId, CancellationToken cancellationToken = default)
    {
        return await _context.VacancyApplicationsReadModels.Where(x => x.VacancyId == vacancyId).ToListAsync(cancellationToken);
    }

    public async Task Remove(Guid vacancyId)
    {
        var modelsToRemove = await _context.VacancyApplicationsReadModels.Where(x => x.VacancyId == vacancyId).ToListAsync();
        if (modelsToRemove.Any())
        {
            _context.VacancyApplicationsReadModels.RemoveRange(modelsToRemove);
        }
    }

    public async Task RemoveByResume(Guid resumeId)
    {
        var modelsToRemove = await _context.VacancyApplicationsReadModels.Where(x => x.ResumeId == resumeId).ToListAsync();
        if (modelsToRemove.Any())
        {
            _context.VacancyApplicationsReadModels.RemoveRange(modelsToRemove);
        }
    }

    public async Task UpdateByResume(Guid resumeId)
    {
        var modelsToUpdate = await _context.VacancyApplicationsReadModels.Where(x => x.ResumeId == resumeId).ToListAsync();
        if (modelsToUpdate.Any())
        {
            var resumeTitle = await _context.Resumes
                .Where(x => x.Id == new ResumeId(resumeId))
                .Select(x => x.DesiredPosition.Title)
                .FirstOrDefaultAsync();

            if (modelsToUpdate[0].ResumeTitle != resumeTitle)
            {
                foreach (var model in modelsToUpdate)
                {
                    model.ResumeTitle = resumeTitle;
                }

                _context.VacancyApplicationsReadModels.UpdateRange(modelsToUpdate);
            }
        }
    }
}
