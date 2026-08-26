using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class ProcessedPaymentOrderConfiguration : IEntityTypeConfiguration<ProcessedPaymentOrder>
{
    public void Configure(EntityTypeBuilder<ProcessedPaymentOrder> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasQueryFilter(o => !o.IsDeleted);

        builder.Property(o => o.OrderId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.ReferenceId)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.ProcessedAt)
            .IsRequired();

        builder.HasIndex(o => o.OrderId)
            .IsUnique();
    }
}
