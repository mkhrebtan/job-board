using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.IDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "Identity");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id).HasConversion(
            refreshTokenId => refreshTokenId.Value,
            value => new RefreshTokenId(value));

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.HasIndex(rt => rt.UserId).IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .IsRequired();

        builder.Property(rt => rt.UserId).HasConversion(
            userId => userId.Value,
            value => new UserId(value));
    }
}
