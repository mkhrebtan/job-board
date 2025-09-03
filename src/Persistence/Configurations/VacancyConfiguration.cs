using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.Enums;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class VacancyConfiguration : IEntityTypeConfiguration<Vacancy>
{
    public void Configure(EntityTypeBuilder<Vacancy> builder)
    {
        builder.ToTable("Vacancies", "JobPosting");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasConversion(
                vacancyId => vacancyId.Value,
                value => new VacancyId(value));

        builder.ComplexProperty<VacancyTitle>(v => v.Title, titleBuilder =>
        {
            titleBuilder.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(VacancyTitle.MaxLength);
        });

        builder.ComplexProperty<RichTextContent>(v => v.Description, descBuilder =>
        {
            descBuilder.Property(d => d.Markdown).IsRequired().HasMaxLength(RichTextContent.MaxLength);
            descBuilder.Property(d => d.PlainText).IsRequired();
        });

        builder.Property(v => v.Status)
            .HasConversion(
                status => status.ToString(),
                value => VacancyStatus.FromCode(value)!)
            .IsRequired();

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(v => v.CategoryId);

        builder.ComplexProperty<Salary>(v => v.Salary, salaryBuilder =>
        {
            salaryBuilder.Property(s => s.Currency).IsRequired();
            salaryBuilder.Property(s => s.MinAmount).IsRequired();
            salaryBuilder.Property(s => s.MaxAmount).IsRequired();
        });

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(v => v.CompanyId)
            .IsRequired();

        builder.ComplexProperty<Location>(r => r.Location, locationBuilder =>
        {
            locationBuilder.Property(l => l.City).IsRequired().HasMaxLength(Location.MaxCityLength);
            locationBuilder.Property(l => l.Country).IsRequired().HasMaxLength(Location.MaxCountryLength);
            locationBuilder.Property(l => l.Region).HasMaxLength(Location.MaxRegionLength);
            locationBuilder.Property(l => l.District).HasMaxLength(Location.MaxDistrictLength);
            locationBuilder.Property(l => l.Address).HasMaxLength(Location.MaxAddressLength);
        });

        builder.ComplexProperty<RecruiterInfo>(v => v.RecruiterInfo, infoBuilder =>
        {
            infoBuilder.Property(i => i.FirstName).IsRequired().HasMaxLength(RecruiterInfo.MaxFirstNameLength);

            infoBuilder.Property(i => i.Email)
            .HasConversion(
                email => email.Address,
                address => Email.Create(address).Value)
            .IsRequired()
            .HasMaxLength(Email.MaxLength);

            infoBuilder.ComplexProperty<PhoneNumber>(i => i.PhoneNumber, phoneBuilder =>
            {
                phoneBuilder.Property(p => p.RegionCode).HasMaxLength(PhoneNumber.RegionCodeLength).IsRequired();
                phoneBuilder.Property(p => p.Number).IsRequired().HasMaxLength(PhoneNumber.MaxNumberLength);
            });
        });
    }
}
