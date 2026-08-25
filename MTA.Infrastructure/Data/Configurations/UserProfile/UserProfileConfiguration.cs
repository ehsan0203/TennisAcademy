using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(profile => profile.Id);
        builder.HasQueryFilter(profile => !profile.IsDeleted);

        builder.Property(profile => profile.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(profile => profile.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(profile => profile.DateOfBirth)
            .IsRequired();

        builder.Property(profile => profile.Experience)
            .IsRequired();

        builder.HasOne(profile => profile.Account)
            .WithOne(account => account.UserProfile)
            .HasForeignKey<UserProfile>(profile => profile.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(profile => profile.SkillLevel)
            .WithMany()
            .HasForeignKey(profile => profile.SkillLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
