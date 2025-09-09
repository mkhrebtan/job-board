using Domain.ReadModels.Resumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Read;

internal sealed class ResumeListingConfiguration : IEntityTypeConfiguration<ResumeListingReadModel>
{
    public void Configure(EntityTypeBuilder<ResumeListingReadModel> builder)
    {
        builder.ToTable("ResumeListing", "Read");

        builder.HasKey(r => r.ResumeId);

        builder.Property(r => r.EmploymentTypes).HasColumnType("text[]");
        builder.Property(r => r.WorkArrangements).HasColumnType("text[]");

        builder.HasIndex(r => r.Country);
        builder.HasIndex(r => r.City);
        builder.HasIndex(r => r.Region);
        builder.HasIndex(r => r.District);
    }
}
