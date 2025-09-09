using Domain.Contexts.JobPostingContext.Enums;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.ReadModels.Vacancies;
using Domain.Repos.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.ReadModels;

internal class CompanyVacanciesReadModelRepository : ICompanyVacanciesReadModelRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyVacanciesReadModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(CompanyVacanciesReadModel model)
    {
        _context.CompanyVacancies.Add(model);
    }

    public async Task<IEnumerable<CompanyVacanciesReadModel>> GetAllByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.CompanyVacancies.Where(x => x.CompanyId == companyId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CompanyVacanciesReadModel>> GetPublishedByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _context.CompanyVacancies.Where(x => x.CompanyId == companyId && x.Status == VacancyStatus.Published.Code).ToListAsync(cancellationToken);
    }

    public async Task Remove(Guid vacancyId)
    {
        var modelToRemove = await _context.CompanyVacancies.FirstOrDefaultAsync(x => x.VacancyId == vacancyId);
        if (modelToRemove != null)
        {
            _context.CompanyVacancies.Remove(modelToRemove);
        }
    }

    public async Task Update(Guid vacancyId)
    {
        var modelToUpdate = await _context.CompanyVacancies.FirstOrDefaultAsync(x => x.VacancyId == vacancyId);
        if (modelToUpdate != null)
        {
            var vacancyData = await _context.Vacancies
                .Where(r => r.Id == new VacancyId(vacancyId))
                .Select(r => new
                {
                    r.CategoryId,
                    Title = r.Title.Value,
                    r.Salary.MinAmount,
                    r.Salary.MaxAmount,
                    r.Salary.Currency,
                    r.Location.Country,
                    r.Location.City,
                    r.Location.Region,
                    r.Location.District,
                    r.LastUpdatedAt,
                    r.Status.Code
                })
                .FirstOrDefaultAsync();

            if (vacancyData != null)
            {
                modelToUpdate.CategoryId = vacancyData.CategoryId is null ? null : vacancyData.CategoryId.Value;
                modelToUpdate.Title = vacancyData.Title;
                modelToUpdate.SalaryFrom = vacancyData.MinAmount;
                modelToUpdate.SalaryTo = vacancyData.MaxAmount;
                modelToUpdate.SalaryCurrency = vacancyData.Currency;
                modelToUpdate.Country = vacancyData.Country;
                modelToUpdate.City = vacancyData.City;
                modelToUpdate.Region = vacancyData.Region;
                modelToUpdate.District = vacancyData.District;
                modelToUpdate.LastUpdatedAt = vacancyData.LastUpdatedAt;
                modelToUpdate.Status = vacancyData.Code;

                _context.CompanyVacancies.Update(modelToUpdate);
            }
        }
    }
}
