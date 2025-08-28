namespace MTA.Domain.Entities;

/// <summary>
/// Junction table for the many-to-many relationship between roles and permissions
/// </summary>
public class PermissionsRole : BaseEntity
{
    /// <summary>
    /// Role ID
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// Permission ID
    /// </summary>
    public int PermissionId { get; set; }
    
    /// <summary>
    /// Navigation property for the role
    /// </summary>
    public virtual Role Role { get; set; } = null!;
    
    /// <summary>
    /// Navigation property for the permission
    /// </summary>
    public virtual Permission Permission { get; set; } = null!;
}
