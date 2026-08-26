using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.HasKey(token => token.Id);
        builder.HasQueryFilter(token => !token.IsDeleted);

        builder.Property(token => token.Token)
            .IsRequired();

        builder.Property(token => token.ExpiresAt)
            .IsRequired();

        builder.HasOne(token => token.Account)
            .WithMany()
            .HasForeignKey(token => token.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
