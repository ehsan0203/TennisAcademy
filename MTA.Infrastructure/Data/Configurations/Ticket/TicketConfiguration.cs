using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(ticket => ticket.Id);
        builder.HasQueryFilter(ticket => !ticket.IsDeleted);

        builder.Property(ticket => ticket.Topic)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(ticket => ticket.Status)
            .WithMany()
            .HasForeignKey(ticket => ticket.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ticket => ticket.Account)
            .WithMany()
            .HasForeignKey(ticket => ticket.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ticket => ticket.Package)
            .WithMany()
            .HasForeignKey(ticket => ticket.PackageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
