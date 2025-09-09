using Domain.ReadModels.Vacancies;
using Domain.Repos.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repos.ReadModels;

internal class RegisteredVacanciesReadModelRepository : IRegisteredVacanciesReadModelRepository
{
    private readonly ApplicationDbContext _context;

    public RegisteredVacanciesReadModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(RegisteredVacanciesReadModel model)
    {
        _context.RegisteredVacancies.Add(model);
    }

    public async Task<IEnumerable<RegisteredVacanciesReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RegisteredVacancies.ToListAsync(cancellationToken);
    }

    public async Task Remove(Guid vacancyId)
    {
        var modelToRemove = await _context.RegisteredVacancies.FirstOrDefaultAsync(x => x.VacancyId == vacancyId);
        if (modelToRemove != null)
        {
            _context.RegisteredVacancies.Remove(modelToRemove);
        }
    }

    public Task Update(Guid vacancyId)
    {
        return Task.CompletedTask;
    }
}
