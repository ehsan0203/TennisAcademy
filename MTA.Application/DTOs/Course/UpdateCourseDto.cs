namespace MTA.Application.DTOs.Course;

/// <summary>
/// Data Transfer Object for updating an existing course
/// </summary>
public class UpdateCourseDto
{
    /// <summary>
    /// Title of the course
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Detailed description of the course content
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// URL to the course icon image
    /// </summary>
    public string? ImageIcon { get; set; }
    
    /// <summary>
    /// URL to the course poster image
    /// </summary>
    public string? Poster { get; set; }
    
    /// <summary>
    /// Course price in the system's currency
    /// </summary>
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Level ID for this course
    /// </summary>
    public int? LevelId { get; set; }
    
    /// <summary>
    /// Status ID for this course
    /// </summary>
    public int? StatusId { get; set; }
}
