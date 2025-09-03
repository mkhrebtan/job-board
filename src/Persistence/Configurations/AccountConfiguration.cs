using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Entities;
using Domain.Contexts.IdentityContext.IDs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", "Identity");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasConversion(
            accountId => accountId.Value,
            value => new AccountId(value));

        builder.HasIndex(a => a.UserId).IsUnique();

        builder.HasOne<User>()
            .WithOne(u => u.Account)
            .HasForeignKey<Account>(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.UserId).HasConversion(
            userId => userId.Value,
            value => new UserId(value));

        builder.Property(a => a.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);
    }
}