using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class MessageMediaFileConfiguration : IEntityTypeConfiguration<MessageMediaFile>
{
    public void Configure(EntityTypeBuilder<MessageMediaFile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Message)
            .WithMany(m => m.MediaFiles)
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MediaFile)
            .WithMany()
            .HasForeignKey(x => x.MediaFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
