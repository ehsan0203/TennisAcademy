using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(token => token.Id);

        builder.Property(token => token.Token)
            .IsRequired();

        builder.Property(token => token.ExpiresAt)
            .IsRequired();

        builder.HasOne(token => token.Account)
            .WithMany(account => account.RefreshTokens)
            .HasForeignKey(token => token.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
