using Application.Services;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.ReadModels.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

internal sealed class CompanyVacanciesReadModelService : ICompanyVacanciesReadModelService
{
    private readonly ApplicationDbContext _context;

    public CompanyVacanciesReadModelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyVacanciesReadModel?> GenerateReadModelAsync(VacancyId VacancyId, CancellationToken cancellationToken = default)
    {
        var vacancyData = await _context.Vacancies
            .Where(v => v.Id == VacancyId)
            .Select(v => new
            {
                CompanyId = v.CompanyId.Value,
                VacancyId = v.Id.Value,
                v.CategoryId,
                Title = v.Title.Value,
                SalaryFrom = v.Salary.MinAmount,
                SalaryTo = v.Salary.MaxAmount,
                SalaryCurrency = v.Salary.Currency,
                v.Location.Country,
                v.Location.City,
                v.Location.Region,
                v.Location.District,
                v.LastUpdatedAt,
                Status = v.Status.Code
            })
            .FirstOrDefaultAsync(CancellationToken.None);

        if (vacancyData == null)
        {
            return null;
        }

        Guid? categoryId = default;
        if (vacancyData.CategoryId is not null)
        {
            categoryId = vacancyData.CategoryId.Value;
        }

        return new CompanyVacanciesReadModel
        {
            CompanyId = vacancyData.CompanyId,
            VacancyId = vacancyData.VacancyId,
            CategoryId = categoryId,
            Title = vacancyData.Title,
            SalaryFrom = vacancyData.SalaryFrom,
            SalaryTo = vacancyData.SalaryTo,
            SalaryCurrency = vacancyData.SalaryCurrency,
            Country = vacancyData.Country,
            City = vacancyData.City,
            Region = vacancyData.Region,
            District = vacancyData.District,
            LastUpdatedAt = vacancyData.LastUpdatedAt,
            Status = vacancyData.Status
        };
    }
}
