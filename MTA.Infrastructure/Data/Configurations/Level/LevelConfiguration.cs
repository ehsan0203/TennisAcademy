using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations.Level;

public class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.HasKey(level => level.Id);

        builder.Property(level => level.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(level => level.Title)
            .IsUnique();
    }
}
