using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data.Configurations;

public class PermissionsRoleConfiguration : IEntityTypeConfiguration<PermissionsRole>
{
    public void Configure(EntityTypeBuilder<PermissionsRole> builder)
    {
        builder.HasKey(permissionsRole => permissionsRole.Id);

        builder.HasOne(permissionsRole => permissionsRole.Role)
            .WithMany(role => role.RolePermissions)
            .HasForeignKey(permissionsRole => permissionsRole.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(permissionsRole => permissionsRole.Permission)
            .WithMany(permission => permission.PermissionsRoles)
            .HasForeignKey(permissionsRole => permissionsRole.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
