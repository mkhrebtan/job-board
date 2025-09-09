using Domain.Contexts.ResumePostingContext.IDs;
using Domain.ReadModels.Resumes;
using Domain.Repos.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.ReadModels;

internal class ResumeListingReadModelRepository : IResumeListingReadModelRepository
{
    private readonly ApplicationDbContext _context;

    public ResumeListingReadModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(ResumeListingReadModel model)
    {
        _context.ResumeListing.Add(model);
    }

    public async Task<IEnumerable<ResumeListingReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ResumeListing.OrderBy(x => x.LastUpdatedAt).ToListAsync(cancellationToken);
    }

    public async Task Remove(Guid resumeId)
    {
        var modelToRemove = await _context.ResumeListing.FirstOrDefaultAsync(x => x.ResumeId == resumeId);
        if (modelToRemove != null)
        {
            _context.Remove(modelToRemove);
        }
    }

    public async Task Update(Guid resumeId)
    {
        var modelToUpdate = await _context.ResumeListing.FirstOrDefaultAsync(x => x.ResumeId == resumeId);
        if (modelToUpdate != null)
        {
            var resumeData = await _context.Resumes
                .Where(r => r.Id == new ResumeId(resumeId))
                .Select(r => new
                {
                    r.DesiredPosition.Title,
                    r.PersonalInfo.FirstName,
                    TotalMonthsOfExperience = r.WorkExperiences.Sum(we => we.DateRange.Duration.Days) / 30,
                    r.SalaryExpectation.Value,
                    r.SalaryExpectation.Currency,
                    EmploymentTypes = r.EmploymentTypes.Select(x => x.Code).ToList(),
                    WorkArrangements = r.WorkArrangements.Select(x => x.Code).ToList(),
                    r.Location.Country,
                    r.Location.City,
                    r.Location.Region,
                    r.Location.District,
                    r.Location.Latitude,
                    r.Location.Longitude,
                    r.LastUpdatedAt
                })
                .FirstOrDefaultAsync();

            if (resumeData != null)
            {
                modelToUpdate.Title = resumeData.Title;
                modelToUpdate.FirstName = resumeData.FirstName;
                modelToUpdate.TotalMonthsOfExperience = resumeData.TotalMonthsOfExperience;
                modelToUpdate.ExpectedSalary = resumeData.Value;
                modelToUpdate.ExpectedSalaryCurrency = resumeData.Currency;
                modelToUpdate.EmploymentTypes = resumeData.EmploymentTypes;
                modelToUpdate.WorkArrangements = resumeData.WorkArrangements;
                modelToUpdate.Country = resumeData.Country;
                modelToUpdate.City = resumeData.City;
                modelToUpdate.Region = resumeData.Region;
                modelToUpdate.District = resumeData.District;
                modelToUpdate.Latitude = resumeData.Latitude;
                modelToUpdate.Longitude = resumeData.Longitude;
                modelToUpdate.LastUpdatedAt = resumeData.LastUpdatedAt;

                _context.ResumeListing.Update(modelToUpdate);
            }
        }
    }
}
