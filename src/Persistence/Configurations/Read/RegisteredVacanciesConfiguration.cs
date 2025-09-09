using Domain.ReadModels.Vacancies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Read;

internal sealed class RegisteredVacanciesConfiguration : IEntityTypeConfiguration<RegisteredVacanciesReadModel>
{
    public void Configure(EntityTypeBuilder<RegisteredVacanciesReadModel> builder)
    {
        builder.ToTable("RegisteredVacancies", "Read");

        builder.HasKey(r => r.VacancyId);
    }
}