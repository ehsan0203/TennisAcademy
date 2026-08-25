using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(account => account.Id);
        builder.HasQueryFilter(account => !account.IsDeleted);

        builder.Property(account => account.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(account => account.Password)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(account => account.ProfileImagePath)
            .HasMaxLength(1000);

        builder.HasIndex(account => account.Email)
            .IsUnique();

        builder.HasOne(account => account.Role)
            .WithMany(role => role.Accounts)
            .HasForeignKey(account => account.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(account => account.Status)
            .WithMany()
            .HasForeignKey(account => account.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(account => account.MediaFile)
            .WithMany()
            .HasForeignKey(account => account.MediaFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
