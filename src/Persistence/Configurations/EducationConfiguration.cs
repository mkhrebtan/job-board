using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Entities;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.ToTable("Educations", "ResumePosting");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasConversion(
            educationId => educationId.Value,
            value => new EducationId(value));

        builder.Property(e => e.InstitutionName)
            .IsRequired()
            .HasMaxLength(Education.MaxInstitutionNameLength);

        builder.Property(e => e.Degree)
            .IsRequired()
            .HasMaxLength(Education.MaxDegreeLength);

        builder.Property(e => e.FieldOfStudy)
            .IsRequired()
            .HasMaxLength(Education.MaxFieldOfStudyLength);

        builder.ComplexProperty<DateRange>(e => e.DateRange, dateRangeBuilder =>
        {
            dateRangeBuilder.Property(dr => dr.StartDate).IsRequired();
            dateRangeBuilder.Property(dr => dr.EndDate);
        });

        builder.HasOne<Resume>()
            .WithMany(r => r.Educations)
            .HasForeignKey(e => e.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
