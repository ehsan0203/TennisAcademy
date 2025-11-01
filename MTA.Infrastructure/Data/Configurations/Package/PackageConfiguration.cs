using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.HasKey(package => package.Id);

        builder.Property(package => package.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(package => package.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(package => package.CreditCount)
            .IsRequired();

        builder.Property(package => package.Duration)
            .IsRequired();

        builder.HasOne(package => package.DurationUnit)
            .WithMany()
            .HasForeignKey(package => package.DurationUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
