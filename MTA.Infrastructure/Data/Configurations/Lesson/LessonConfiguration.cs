using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(lesson => lesson.Id);

        builder.Property(lesson => lesson.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(lesson => lesson.Description)
            .HasMaxLength(2000);

        builder.HasOne(lesson => lesson.Course)
            .WithMany(course => course.Lessons)
            .HasForeignKey(lesson => lesson.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lesson => lesson.MediaFile)
            .WithMany()
            .HasForeignKey(lesson => lesson.MediaFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
