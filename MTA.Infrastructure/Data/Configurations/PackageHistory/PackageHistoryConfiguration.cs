using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class PackageHistoryConfiguration : IEntityTypeConfiguration<PackageHistory>
{
    public void Configure(EntityTypeBuilder<PackageHistory> builder)
    {
        builder.HasKey(history => history.Id);

        builder.Property(history => history.ExpiredDate)
            .IsRequired();

        builder.Property(history => history.RemainingTickets)
            .IsRequired();

        builder.Property(history => history.RemainingMessages)
            .IsRequired();

        builder.HasOne(history => history.Package)
            .WithMany(package => package.PackageHistories)
            .HasForeignKey(history => history.PackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(history => history.Account)
            .WithMany(account => account.PackageHistory)
            .HasForeignKey(history => history.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
