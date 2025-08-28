namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Role entity
/// </summary>
public class RoleDto : BaseDto
{
    /// <summary>
    /// Title of the role
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Number of accounts with this role
    /// </summary>
    public int AccountCount { get; set; }
    
    /// <summary>
    /// Number of permissions assigned to this role
    /// </summary>
    public int PermissionCount { get; set; }
}
