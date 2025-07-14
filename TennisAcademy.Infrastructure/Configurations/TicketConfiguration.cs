using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Infrastructure.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Subject)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(t => t.Description)
                   .IsRequired();

            builder.Property(t => t.FileUrl)
                   .HasMaxLength(500);

            builder.Property(t => t.VoiceUrl)
                   .HasMaxLength(500);

            builder.Property(t => t.CoachReplyVoiceUrl)
                   .HasMaxLength(500);

            builder.Property(t => t.CoachReplyVideoUrl)
                   .HasMaxLength(500);

            builder.Property(t => t.CoachReply)
                   .HasMaxLength(1000);

            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("getutcdate()");

            builder.HasOne(t => t.User)
                   .WithMany(u => u.Tickets)
                   .HasForeignKey(t => t.UserId);

            builder.HasOne(t => t.Coach)
                   .WithMany()
                   .HasForeignKey(t => t.CoachId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
