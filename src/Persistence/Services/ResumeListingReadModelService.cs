using Application.Services;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.ReadModels.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

internal class ResumeListingReadModelService : IResumeListingReadModelService
{
    private readonly ApplicationDbContext _context;

    public ResumeListingReadModelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResumeListingReadModel?> GenerateReadModelAsync(ResumeId ResumeId, CancellationToken cancellationToken = default)
    {
        var resumeData = await _context.Resumes
            .Where(r => r.Id == ResumeId)
            .Select(r => new
            {
                ResumeId = r.Id.Value,
                Title = r.DesiredPosition.Title,
                FirstName = r.PersonalInfo.FirstName,
                WorkExperiences = r.WorkExperiences,
                ExpectedSalary = r.SalaryExpectation.Value,
                ExpectedSalaryCurrency = r.SalaryExpectation.Currency,
                EmploymentTypes = r.EmploymentTypes,
                WorkArrangements = r.WorkArrangements,
                Country = r.Location.Country,
                City = r.Location.City,
                Region = r.Location.Region,
                District = r.Location.District,
                Latitude = r.Location.Latitude,
                Longitude = r.Location.Longitude,
                LastUpdatedAt = r.LastUpdatedAt
            })
            .FirstOrDefaultAsync(CancellationToken.None);

        if (resumeData is null)
        {
            return null;
        }

        DateTime time = DateTime.UtcNow;
        int months = resumeData.WorkExperiences.Select(x => new
        {
            StartDate = x.DateRange.StartDate,
            EndDate = x.DateRange.EndDate ?? time,
        })
        .Sum(x => ((x.EndDate.Year - x.StartDate.Year) * 12) + (x.EndDate.Month - x.StartDate.Month));

        return new ResumeListingReadModel
        {
            ResumeId = resumeData!.ResumeId,
            Title = resumeData.Title,
            FirstName = resumeData.FirstName,
            TotalMonthsOfExperience = months,
            ExpectedSalary = resumeData.ExpectedSalary,
            ExpectedSalaryCurrency = resumeData.ExpectedSalaryCurrency,
            EmploymentTypes = [.. resumeData.EmploymentTypes.Select(x => x.Code)],
            WorkArrangements = [.. resumeData.WorkArrangements.Select(x => x.Code)],
            Country = resumeData.Country,
            City = resumeData.City,
            Region = resumeData.Region,
            District = resumeData.District,
            Latitude = resumeData.Latitude,
            Longitude = resumeData.Longitude,
            LastUpdatedAt = resumeData.LastUpdatedAt
        };
    }
}
