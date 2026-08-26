using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class FooterContactItemConfiguration : IEntityTypeConfiguration<FooterContactItem>
{
    public void Configure(EntityTypeBuilder<FooterContactItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.Property(i => i.Label)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Value)
            .IsRequired()
            .HasMaxLength(500);
    }
}
