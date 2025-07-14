using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Infrastructure.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PurchaseDate)
                   .HasDefaultValueSql("getutcdate()");

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Purchases)
                   .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.Course)
                   .WithMany(c => c.Purchases)
                   .HasForeignKey(p => p.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Plan)
                   .WithMany(pl => pl.Purchases)
                   .HasForeignKey(p => p.PlanId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
