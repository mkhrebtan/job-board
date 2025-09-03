using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.IDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", "JobPosting");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                categoryId => categoryId.Value,
                value => new CategoryId(value));

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(Category.MaxNameLength);

        builder.HasIndex(c => c.NormalizedName)
            .IsUnique();

        builder.Property(c => c.NormalizedName)
            .IsRequired()
            .HasMaxLength(Category.MaxNameLength);
    }
}
