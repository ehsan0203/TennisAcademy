using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Infrastructure.Configurations
{
    public class CourseVideoConfiguration : IEntityTypeConfiguration<CourseVideo>
    {
        public void Configure(EntityTypeBuilder<CourseVideo> builder)
        {
            builder.HasKey(cv => cv.Id);

            builder.Property(cv => cv.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(cv => cv.VideoUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(cv => cv.Course)
                   .WithMany(c => c.Videos)
                   .HasForeignKey(cv => cv.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
