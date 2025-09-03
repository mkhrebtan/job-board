using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Entities;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class LanguageSkillConfiguration : IEntityTypeConfiguration<LanguageSkill>
{
    public void Configure(EntityTypeBuilder<LanguageSkill> builder)
    {
        builder.ToTable("LanguageSkills", "ResumePosting");

        builder.HasKey(ls => ls.Id);
        builder.Property(ls => ls.Id).HasConversion(
            languageSkillId => languageSkillId.Value,
            value => new LanguageId(value));

        builder.Property(ls => ls.Language)
            .IsRequired()
            .HasMaxLength(LanguageSkill.MaxLanguageLength);

        builder.Property(ls => ls.ProficiencyLevel)
            .HasConversion(
                proficiency => proficiency.ToString(),
                value => LanguageLevel.FromCode(value)!)
            .IsRequired();

        builder.HasOne<Resume>()
            .WithMany(r => r.Languages)
            .HasForeignKey(ls => ls.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
