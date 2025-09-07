using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Entities;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Persistence.Configurations;

internal sealed class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.ToTable("Resumes", "ResumePosting");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).HasConversion(
            resumeId => resumeId.Value,
            value => new ResumeId(value));

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.SeekerId)
            .IsRequired();

        builder.ComplexProperty<PersonalInfo>(r => r.PersonalInfo, personalInfoBuilder =>
        {
            personalInfoBuilder.Property(pi => pi.FirstName).IsRequired().HasMaxLength(PersonalInfo.MaxFirstNameLength);
            personalInfoBuilder.Property(pi => pi.LastName).IsRequired().HasMaxLength(PersonalInfo.MaxLastNameLength);
        });

        builder.ComplexProperty<Location>(r => r.Location, locationBuilder =>
        {
            locationBuilder.Property(l => l.City).IsRequired().HasMaxLength(Location.MaxCityLength);
            locationBuilder.Property(l => l.Country).IsRequired().HasMaxLength(Location.MaxCountryLength);
            locationBuilder.Property(l => l.Region).HasMaxLength(Location.MaxRegionLength);
            locationBuilder.Property(l => l.District).HasMaxLength(Location.MaxDistrictLength);
            locationBuilder.Property(l => l.Address).HasMaxLength(Location.MaxAddressLength);
        });

        builder.ComplexProperty<ContactInfo>(r => r.ContactInfo, contactInfoBuilder =>
        {
            contactInfoBuilder.Property(ci => ci.Email)
            .HasConversion(
                email => email.Address,
                address => Email.Create(address).Value)
            .IsRequired()
            .HasMaxLength(Email.MaxLength);

            contactInfoBuilder.ComplexProperty<PhoneNumber>(ci => ci.PhoneNumber, phoneBuilder =>
            {
                phoneBuilder.Property(p => p.RegionCode).HasMaxLength(PhoneNumber.RegionCodeLength).IsRequired();
                phoneBuilder.Property(p => p.Number).IsRequired().HasMaxLength(PhoneNumber.MaxNumberLength);
            });
        });

        builder.Property(r => r.DesiredPosition)
            .HasConversion(
                title => title.Title,
                value => DesiredPosition.Create(value).Value);

        builder.ComplexProperty<Money>(r => r.SalaryExpectation, salaryBuilder =>
        {
            salaryBuilder.Property(s => s.Currency).IsRequired();
            salaryBuilder.Property(s => s.Value).IsRequired();
        });

        builder.ComplexProperty<RichTextContent>(r => r.SkillsDescription, summaryBuilder =>
        {
            summaryBuilder.Property(s => s.Markdown).IsRequired().HasMaxLength(RichTextContent.MaxLength);
            summaryBuilder.Property(s => s.PlainText).IsRequired();
        });

        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.LastUpdatedAt).IsRequired();

        builder.Property<ResumeStatus>(r => r.Status)
            .HasConversion(
            status => status.ToString(),
            value => ResumeStatus.FromCode(value)!);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var employmentConverter = new ValueConverter<IReadOnlyCollection<EmploymentType>, string>(
            types => JsonSerializer
                .Serialize(types.Select(t => t.Code), jsonOptions),
            json => JsonSerializer
                .Deserialize<string[]>(json, jsonOptions)!
                .Select(code => EmploymentType.FromCode(code))!
                .ToHashSet<EmploymentType>());

        builder.Property(r => r.EmploymentTypes)
            .HasConversion(employmentConverter)
            .HasColumnType("jsonb");

        var arrangementConverter = new ValueConverter<IReadOnlyCollection<WorkArrangement>, string>(
            types => JsonSerializer
                .Serialize(types.Select(t => t.Code), jsonOptions),
            json => JsonSerializer
                .Deserialize<string[]>(json, jsonOptions)!
                .Select(code => WorkArrangement.FromCode(code))!
                .ToHashSet<WorkArrangement>());

        builder.Property(r => r.WorkArrangements)
            .HasConversion(arrangementConverter)
            .HasColumnType("jsonb");

        builder.HasMany(r => r.Educations)
            .WithOne()
            .HasForeignKey(e => e.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Languages)
            .WithOne()
            .HasForeignKey(ls => ls.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.WorkExperiences)
            .WithOne()
            .HasForeignKey(we => we.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
