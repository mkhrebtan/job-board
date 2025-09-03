using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Entities;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "Identity");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasConversion(
            userId => userId.Value,
            value => new UserId(value));

        builder.Property(u => u.FirstName)
           .IsRequired()
           .HasMaxLength(User.MaxFirstNameLength);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(User.MaxLastNameLength);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Address,
                address => Email.Create(address).Value)
            .HasMaxLength(Email.MaxLength);

        builder.ComplexProperty<PhoneNumber>(u => u.PhoneNumber, numberBuilder =>
        {
            numberBuilder.Property(n => n.RegionCode).HasMaxLength(PhoneNumber.RegionCodeLength).IsRequired();
            numberBuilder.Property(n => n.Number).IsRequired().HasMaxLength(PhoneNumber.MaxNumberLength);
        });

        builder.HasOne(u => u.Account)
            .WithOne()
            .HasForeignKey<Account>(a => a.UserId);

        builder.Property(u => u.Role)
            .HasConversion(
                role => role.ToString(),
                value => UserRole.FromCode(value)!)
            .IsRequired();
    }
}
