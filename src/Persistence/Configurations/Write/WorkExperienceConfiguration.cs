using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Entities;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Write;

internal sealed class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    public void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        builder.ToTable("WorkExperiences", "ResumePosting");

        builder.HasKey(we => we.Id);
        builder.Property(we => we.Id).HasConversion(
            workExperienceId => workExperienceId.Value,
            value => new WorkExperienceId(value));

        builder.Property(we => we.CompanyName)
            .IsRequired()
            .HasMaxLength(WorkExperience.MaxCompanyNameLength);

        builder.Property(we => we.Position)
            .IsRequired()
            .HasMaxLength(WorkExperience.MaxPositionLength);

        builder.ComplexProperty(we => we.DateRange, dateRangeBuilder =>
        {
            dateRangeBuilder.Property(dr => dr.StartDate).IsRequired();
            dateRangeBuilder.Property(dr => dr.EndDate);
        });

        builder.ComplexProperty(we => we.Description, respBuilder =>
        {
            respBuilder.Property(r => r.Markdown).IsRequired().HasMaxLength(RichTextContent.MaxLength);
            respBuilder.Property(r => r.PlainText).IsRequired();
        });

        builder.HasOne<Resume>()
            .WithMany(r => r.WorkExperiences)
            .HasForeignKey(we => we.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
