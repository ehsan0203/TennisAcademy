using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class LookupConfiguration : IEntityTypeConfiguration<Lookup>
{
    public void Configure(EntityTypeBuilder<Lookup> builder)
    {
        builder.HasKey(lookup => lookup.Id);
        builder.HasQueryFilter(lookup => !lookup.IsDeleted);

        builder.Property(lookup => lookup.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(lookup => lookup.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(lookup => lookup.Value)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(lookup => new { lookup.Category, lookup.Key })
            .IsUnique();
    }
}
