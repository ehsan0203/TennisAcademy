using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class SiteImageConfiguration : IEntityTypeConfiguration<SiteImage>
{
    public void Configure(EntityTypeBuilder<SiteImage> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.Property(i => i.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Url)
            .HasMaxLength(1000);

        builder.HasIndex(i => i.Key)
            .IsUnique();
    }
}
