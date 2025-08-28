namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Level entity
/// </summary>
public class LevelDto : BaseDto
{
    /// <summary>
    /// Title of the level
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Number of courses available at this level
    /// </summary>
    public int CourseCount { get; set; }
    
    /// <summary>
    /// Number of users with this skill level
    /// </summary>
    public int UserCount { get; set; }
}
