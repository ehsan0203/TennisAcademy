using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations.Course;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(course => course.Id);

        builder.Property(course => course.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(course => course.Description)
            .HasMaxLength(2000);

        builder.Property(course => course.Price)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(course => course.IconMediaFile)
            .WithMany()
            .HasForeignKey(course => course.IconMediaFileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(course => course.PosterMediaFile)
            .WithMany()
            .HasForeignKey(course => course.PosterMediaFileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(course => course.Level)
            .WithMany(level => level.Courses)
            .HasForeignKey(course => course.LevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(course => course.Status)
            .WithMany()
            .HasForeignKey(course => course.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
