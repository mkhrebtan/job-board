using Application.Services;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.ReadModels.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

internal sealed class RegisteredVacanciesReadModelService : IRegisteredVacanciesReadModelService
{
    private readonly ApplicationDbContext _context;

    public RegisteredVacanciesReadModelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RegisteredVacanciesReadModel?> GenerateReadModelAsync(VacancyId VacancyId, UserId userId, CancellationToken cancellationToken = default)
    {
        var vacancyData =
            from vacancy in _context.Vacancies
            join company in _context.Companies on vacancy.CompanyId equals company.Id
            join user in _context.Users on userId equals user.Id
            where vacancy.Id == VacancyId
            select new
            {
                Id = vacancy.Id,
                Title = vacancy.Title,
                CompanyName = company.Name,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserEmail = user.Email,
                UserPhoneNumber = user.PhoneNumber,
                RegisteredAt = vacancy.RegisteredAt
            };

        var modelData = await vacancyData.FirstOrDefaultAsync(CancellationToken.None);

        return modelData is not null
            ? new RegisteredVacanciesReadModel(
                modelData.Id.Value,
                modelData.Title.Value,
                modelData.CompanyName,
                modelData.FirstName + " " + modelData.LastName,
                modelData.UserEmail.Address,
                modelData.UserPhoneNumber.Number,
                (DateTime)modelData.RegisteredAt!)
            : null;
    }
}
