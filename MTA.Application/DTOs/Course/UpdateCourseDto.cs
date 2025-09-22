using Microsoft.AspNetCore.Http;

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
    /// MediaFile ID for the course icon
    /// </summary>
    public int? IconMediaFileId { get; set; }

    /// <summary>
    /// MediaFile ID for the course poster
    /// </summary>
    public int? PosterMediaFileId { get; set; }

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

    public IFormFile? NewPoster { get; set; }

    public IFormFile? NewIcon { get; set; }
}
