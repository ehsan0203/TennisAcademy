namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for RolePermission entity
/// </summary>
public class RolePermissionDto : BaseDto
{
    /// <summary>
    /// Role ID
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// Role title
    /// </summary>
    public string? RoleTitle { get; set; }
    
    /// <summary>
    /// Permission ID
    /// </summary>
    public int PermissionId { get; set; }
    
    /// <summary>
    /// Permission title
    /// </summary>
    public string? PermissionTitle { get; set; }
    
    /// <summary>
    /// Permission description
    /// </summary>
    public string? PermissionDescription { get; set; }
}
