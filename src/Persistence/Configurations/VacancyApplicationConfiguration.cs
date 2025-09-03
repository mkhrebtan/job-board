using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.ApplicationContext.IDs;
using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.JobPostingContext.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class VacancyApplicationConfiguration : IEntityTypeConfiguration<VacancyApplication>
{
    public void Configure(EntityTypeBuilder<VacancyApplication> builder)
    {
        builder.ToTable("VacancyApplications", "Application");

        builder.HasKey(va => va.Id);

        builder.Property(va => va.Id).HasConversion(
            applicationId => applicationId.Value,
            value => new VacancyApplicationId(value));

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(va => va.SeekerId)
            .IsRequired();

        builder.HasOne<Vacancy>()
            .WithMany()
            .HasForeignKey(va => va.VacancyId)
            .IsRequired();

        builder.HasIndex(va => new { va.SeekerId, va.VacancyId }).IsUnique();

        builder.Property(va => va.CoverLetter)
            .HasConversion(
                letter => letter.Content,
                value => CoverLetter.Create(value).Value)
            .IsRequired()
            .HasMaxLength(CoverLetter.MaxLength);

        builder.HasDiscriminator<string>("ApplicationType")
            .HasValue<ResumeVacancyApplication>("WithCreatedResume")
            .HasValue<FileVacancyApplication>("WithFile");
    }
}
