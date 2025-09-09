using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Write;

internal sealed class ResumeVacancyApplicationConfiguration : IEntityTypeConfiguration<ResumeVacancyApplication>
{
    public void Configure(EntityTypeBuilder<ResumeVacancyApplication> builder)
    {
        builder.HasOne<Resume>()
            .WithMany()
            .HasForeignKey(rva => rva.ResumeId)
            .IsRequired();
    }
}