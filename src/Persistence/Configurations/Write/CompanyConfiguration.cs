using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Contexts.RecruitmentContext.ValueObjects;
using Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Write;

internal sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies", "Recruitment");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                companyId => companyId.Value,
                value => new CompanyId(value));

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(Company.MaxNameLength);

        builder.ComplexProperty(c => c.Description, descBuilder =>
        {
            descBuilder.Property(d => d.Markdown).IsRequired();
            descBuilder.Property(d => d.PlainText).IsRequired();
        });

        builder.Property(c => c.WebsiteUrl)
            .HasConversion(
                url => url.Value,
                value => WebsiteUrl.Create(value).Value)
            .IsRequired();

        builder.Property(c => c.LogoUrl)
            .HasConversion(
                url => url.Value,
                value => LogoUrl.Create(value).Value)
            .IsRequired();

        builder.Property(c => c.IsVerified).IsRequired();
    }
}
