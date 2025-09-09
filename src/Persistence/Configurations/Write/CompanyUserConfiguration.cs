using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Write;

internal sealed class CompanyUserConfiguration : IEntityTypeConfiguration<CompanyUser>
{
    public void Configure(EntityTypeBuilder<CompanyUser> builder)
    {
        builder.ToTable("CompanyUsers", "Recruitment");

        builder.HasKey(cu => cu.Id);

        builder.Property(cu => cu.Id).HasConversion(
            companyUserId => companyUserId.Value,
            value => new CompanyUserId(value));

        builder.HasIndex(cu => cu.RecruiterId).IsUnique();

        builder.Property(cu => cu.CompanyId)
            .HasConversion(
                companyId => companyId.Value,
                value => new CompanyId(value));

        builder.Property(cu => cu.RecruiterId)
            .HasConversion(
                userId => userId.Value,
                value => new UserId(value));

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(cu => cu.CompanyId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(cu => cu.RecruiterId)
            .IsRequired();
    }
}
