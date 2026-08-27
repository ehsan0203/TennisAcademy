using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class SiteTextConfiguration : IEntityTypeConfiguration<SiteText>
{
    public void Configure(EntityTypeBuilder<SiteText> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.Property(t => t.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Value)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(t => t.Key)
            .IsUnique();
    }
}
