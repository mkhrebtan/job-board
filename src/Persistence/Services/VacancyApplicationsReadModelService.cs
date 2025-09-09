using Application.Services;
using Domain.Contexts.ApplicationContext.IDs;
using Domain.ReadModels.Vacancies;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Services;

internal sealed class VacancyApplicationsReadModelService : IVacancyApplicationsReadModelService
{
    private readonly ApplicationDbContext _context;

    public VacancyApplicationsReadModelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacancyApplicationsReadModel?> GenerateReadModelAsync(VacancyApplicationId vacancyApplicationId, CancellationToken cancellationToken = default)
    {
        Guid id = vacancyApplicationId.Value;
        var vacancyApplicationData = _context.VacancyApplicationsReadModels.FromSqlRaw(
            @"
            SELECT
                va.""Id"" AS ""VacancyApplicationId"",
                va.""VacancyId"" AS ""VacancyId"",
                u.""FirstName"" AS ""SeekerFirstName"",
                u.""LastName"" AS ""SeekerLastName"",
                va.""CoverLetter"" AS ""CoverLetter"",
                va.""ResumeId"" As ""ResumeId"",
                r.""DesiredPosition"" AS ""ResumeTitle"",
                va.""FileUrl"" AS ""FileUrl"",
                NOW() AT TIME ZONE 'UTC' AS ""AppliedAt""
            FROM ""Application"".""VacancyApplications"" va
            JOIN ""Identity"".""Users"" u ON va.""SeekerId"" = u.""Id""
            JOIN ""ResumePosting"".""Resumes"" r ON va.""ResumeId"" = r.""Id""
            WHERE va.""Id"" = {0}", id);

        var result = await vacancyApplicationData.FirstOrDefaultAsync(cancellationToken);
        if (result == null)
        {
            return null;
        }

        result.AppliedAt = DateTime.UtcNow;
        return result;
    }
}
