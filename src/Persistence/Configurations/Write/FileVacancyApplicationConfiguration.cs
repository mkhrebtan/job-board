using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.ApplicationContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Write;

internal sealed class FileVacancyApplicationConfiguration : IEntityTypeConfiguration<FileVacancyApplication>
{
    public void Configure(EntityTypeBuilder<FileVacancyApplication> builder)
    {
        builder.Property(fva => fva.FileUrl)
            .HasConversion(
                url => url.Value,
                value => FileUrl.Create(value).Value)
            .IsRequired()
            .HasMaxLength(Domain.Shared.ValueObjects.Url.MaxLength);
    }
}
