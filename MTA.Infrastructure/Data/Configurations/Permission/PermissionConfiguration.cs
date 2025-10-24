using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(permission => permission.Description)
            .HasMaxLength(500);

        builder.HasIndex(permission => permission.Title)
            .IsUnique();
    }
}
