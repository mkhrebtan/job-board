using Domain.Contexts.JobPostingContext.IDs;
using Domain.ReadModels.Vacancies;
using Domain.Repos.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.ReadModels;

internal class VacancyListingReadModelRepository : IVacancyListingReadModelRepository
{
    private readonly ApplicationDbContext _context;

    public VacancyListingReadModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(VacancyListingReadModel model)
    {
        _context.VacancyListing.Add(model);
    }

    public async Task<IEnumerable<VacancyListingReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.VacancyListing.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<VacancyListingReadModel>> GetAllByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.VacancyListing.Where(x => x.CategoryId == categoryId).ToListAsync(cancellationToken);
    }

    public async Task Remove(Guid vacancyId)
    {
        var modelToRemove = await _context.VacancyListing.FirstOrDefaultAsync(x => x.VacancyId == vacancyId);
        if (modelToRemove != null)
        {
            _context.VacancyListing.Remove(modelToRemove);
        }
    }

    public async Task Update(Guid vacancyId)
    {
        var modelToUpdate = await _context.VacancyListing.FirstOrDefaultAsync(x => x.VacancyId == vacancyId);
        if (modelToUpdate != null)
        {
            var vacancy = await _context.Vacancies
                .Where(x => x.Id == new VacancyId(vacancyId))
                .Select(vacancy => new
                {
                    Title = vacancy.Title.Value,
                    SalaryFrom = vacancy.Salary.MinAmount,
                    SalaryTo = vacancy.Salary.MaxAmount,
                    SalaryCurrency = vacancy.Salary.Currency,
                    Description = vacancy.Description.PlainText,
                    Country = vacancy.Location.Country,
                    City = vacancy.Location.City,
                    Region = vacancy.Location.Region,
                    District = vacancy.Location.District,
                    LastUpdatedAt = vacancy.LastUpdatedAt
                })
                .FirstOrDefaultAsync();

            if (vacancy != null)
            {
                modelToUpdate.Title = vacancy.Title;
                modelToUpdate.SalaryFrom = vacancy.SalaryFrom;
                modelToUpdate.SalaryTo = vacancy.SalaryTo;
                modelToUpdate.SalaryCurrency = vacancy.SalaryCurrency;
                modelToUpdate.Country = vacancy.Country;
                modelToUpdate.City = vacancy.City;
                modelToUpdate.Region = vacancy.Region;
                modelToUpdate.District = vacancy.District;
                modelToUpdate.LastUpdatedAt = vacancy.LastUpdatedAt;

                _context.VacancyListing.Update(modelToUpdate);
            }
        }
    }
}
