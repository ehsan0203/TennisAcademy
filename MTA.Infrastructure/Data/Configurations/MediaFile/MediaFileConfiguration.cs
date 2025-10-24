using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations.MediaFile;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.HasKey(mediaFile => mediaFile.Id);

        builder.Property(mediaFile => mediaFile.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(mediaFile => mediaFile.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(mediaFile => mediaFile.Type)
            .WithMany()
            .HasForeignKey(mediaFile => mediaFile.TypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
