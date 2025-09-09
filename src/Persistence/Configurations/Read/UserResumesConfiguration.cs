using Domain.ReadModels.Resumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Read;

internal sealed class UserResumesConfiguration : IEntityTypeConfiguration<UserResumesReadModel>
{
    public void Configure(EntityTypeBuilder<UserResumesReadModel> builder)
    {
        builder.ToTable("UserResumes", "Read");

        builder.HasKey(ur => ur.Id);

        builder.HasIndex(ur => ur.UserId);

        builder.HasIndex(ur => ur.ResumeId).IsUnique();
    }
}
