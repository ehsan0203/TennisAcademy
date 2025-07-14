using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Infrastructure.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.Description)
                   .IsRequired();

            builder.Property(c => c.VideoIntroUrl)
                   .HasMaxLength(500);

            builder.Property(c => c.CoverImageUrl)
                   .HasMaxLength(500);

            builder.Property(c => c.Price)
                   .HasColumnType("decimal(18,2)");

            builder.Property(c => c.IsActive)
                   .HasDefaultValue(true);
        }
    }
}
