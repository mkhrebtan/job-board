using Domain.ReadModels.Vacancies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Read;

internal sealed class VacancyApplicationsReadModelConfiguration : IEntityTypeConfiguration<VacancyApplicationsReadModel>
{

    public void Configure(EntityTypeBuilder<VacancyApplicationsReadModel> builder)
    {
        builder.ToTable("VacancyApplications", "Read");

        builder.HasKey(r => r.VacancyApplicationId);

        builder.HasIndex(r => r.VacancyId);
        builder.HasIndex(r => r.ResumeId);
    }
}