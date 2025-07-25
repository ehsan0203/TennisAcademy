using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Infrastructure.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Description)
                   .IsRequired();

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Credit)
                   .IsRequired();
        }
    }
}
