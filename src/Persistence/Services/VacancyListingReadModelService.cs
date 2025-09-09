using Application.Services;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.ReadModels.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

internal sealed class VacancyListingReadModelService : IVacancyListingReadModelService
{
    private readonly ApplicationDbContext _context;

    public VacancyListingReadModelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacancyListingReadModel?> GenerateReadModelAsync(VacancyId VacancyId, CancellationToken cancellationToken = default)
    {
        var vacancyData =
            from vacancy in _context.Vacancies
            join company in _context.Companies on vacancy.CompanyId equals company.Id
            where vacancy.Id == VacancyId
            select new VacancyListingReadModel
            {
                VacancyId = vacancy.Id.Value,
                CategoryId = vacancy.CategoryId!.Value,
                Title = vacancy.Title.Value,
                CompanyName = company.Name,
                CompanyLogoUrl = company.LogoUrl.Value,
                SalaryFrom = vacancy.Salary.MinAmount,
                SalaryTo = vacancy.Salary.MaxAmount,
                SalaryCurrency = vacancy.Salary.Currency,
                DescriptionPlainText = vacancy.Description.PlainText,
                Country = vacancy.Location.Country,
                City = vacancy.Location.City,
                Region = vacancy.Location.Region,
                District = vacancy.Location.District,
                LastUpdatedAt = vacancy.LastUpdatedAt
            };

        return await vacancyData.FirstOrDefaultAsync(CancellationToken.None);
    }
}
