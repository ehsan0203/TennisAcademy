using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Infrastructure.Configurations
{
    public class CoachMediaConfiguration : IEntityTypeConfiguration<CoachMedia>
    {
        public void Configure(EntityTypeBuilder<CoachMedia> builder)
        {
            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(cm => cm.FileUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(cm => cm.Coach)
                   .WithMany()
                   .HasForeignKey(cm => cm.CoachId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
