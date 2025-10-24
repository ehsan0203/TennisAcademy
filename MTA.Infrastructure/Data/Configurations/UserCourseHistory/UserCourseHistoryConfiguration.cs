using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations.UserCourseHistory;

public class UserCourseHistoryConfiguration : IEntityTypeConfiguration<UserCourseHistory>
{
    public void Configure(EntityTypeBuilder<UserCourseHistory> builder)
    {
        builder.HasKey(history => history.Id);

        builder.HasOne(history => history.Course)
            .WithMany(course => course.UserCourseHistory)
            .HasForeignKey(history => history.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(history => history.Account)
            .WithMany(account => account.UserCourseHistory)
            .HasForeignKey(history => history.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
