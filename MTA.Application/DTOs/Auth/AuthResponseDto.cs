using Microsoft.AspNetCore.Http;

namespace MTA.Application.DTOs.Auth;

/// <summary>
/// Data Transfer Object for authentication response
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// JWT refresh token
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public required UserInfoDto User { get; set; }
}

/// <summary>
/// Data Transfer Object for user information in auth response
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// User's unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User's first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// User's role ID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// User's role title
    /// </summary>
    public required string RoleTitle { get; set; }

    /// <summary>
    /// User's skill level ID
    /// </summary>
    public int SkillLevelId { get; set; }

    /// <summary>
    /// User's skill level value
    /// </summary>
    public string? SkillLevelValue { get; set; }

    /// <summary>
    /// User's profile image URL
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// User's experience in years
    /// </summary>
    public int Experience { get; set; }
    
    /// <summary>
    /// User's date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }
}

public class UpdateFullUserProfileDto
{
    public string? Email { get; set; }
    public bool? IsActive { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Experience { get; set; }
    public bool? HealthCondition { get; set; }
    public string? HealthDescription { get; set; }
    public int? SkillLevelId { get; set; }

    public IFormFile? AvatarFile { get; set; }
}

