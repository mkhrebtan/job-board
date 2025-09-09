using Domain.ReadModels.Vacancies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Read;

internal sealed class CompayVacanciesConfiguration : IEntityTypeConfiguration<CompanyVacanciesReadModel>
{
    public void Configure(EntityTypeBuilder<CompanyVacanciesReadModel> builder)
    {
        builder.ToTable("CompanyVacancies", "Read");

        builder.HasKey(r => r.VacancyId);

        builder.HasIndex(r => r.CompanyId);
        builder.HasIndex(r => r.CategoryId);

        builder.HasIndex(r => r.Status);

        builder.HasIndex(r => r.Country);
        builder.HasIndex(r => r.City);
        builder.HasIndex(r => r.Region);
        builder.HasIndex(r => r.District);
    }
}
