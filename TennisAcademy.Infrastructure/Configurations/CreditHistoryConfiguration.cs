using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Infrastructure.Configurations
{
    public class CreditHistoryConfiguration : IEntityTypeConfiguration<CreditHistory>
    {
        public void Configure(EntityTypeBuilder<CreditHistory> builder)
        {
            builder.HasKey(ch => ch.Id);

            builder.Property(ch => ch.Amount)
                   .IsRequired();

            builder.Property(ch => ch.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(ch => ch.CreatedAt)
                   .IsRequired();

            builder.HasOne(ch => ch.User)
                   .WithMany()
                   .HasForeignKey(ch => ch.UserId);
        }
    }
}
