using Domain.ReadModels.Vacancies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Read;

internal sealed class VacancyListingConfiguration : IEntityTypeConfiguration<VacancyListingReadModel>
{
    public void Configure(EntityTypeBuilder<VacancyListingReadModel> builder)
    {
        builder.ToTable("VacancyListing", "Read");

        builder.HasKey(r => r.VacancyId);

        builder.HasIndex(r => r.CategoryId);
        builder.HasIndex(r => r.Country);
        builder.HasIndex(r => r.City);
        builder.HasIndex(r => r.Region);
        builder.HasIndex(r => r.District);
    }
}