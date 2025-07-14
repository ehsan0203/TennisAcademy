using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Infrastructure.Configurations
{
    public class UserScoreConfiguration : IEntityTypeConfiguration<UserScore>
    {
        public void Configure(EntityTypeBuilder<UserScore> builder)
        {
            builder.HasKey(us => us.Id);

            builder.Property(us => us.Credit)
                   .IsRequired();

            builder.HasOne(us => us.User)
                   .WithOne(u => u.UserScore)
                   .HasForeignKey<UserScore>(us => us.UserId);
        }
    }
}
