using Microsoft.AspNetCore.Http;

namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Lesson entity
/// </summary>
public class LessonDto : BaseDto
{
	/// <summary>
	/// Title of the lesson
	/// </summary>
	public required string Title { get; set; }
	
	/// <summary>
	/// Detailed description of the lesson content
	/// </summary>
	public string? Description { get; set; }
	
	/// <summary>
	/// Whether this lesson is free or requires purchase
	/// </summary>
	public bool IsFree { get; set; }
	
	/// <summary>
	/// Course ID that this lesson belongs to
	/// </summary>
	public int CourseId { get; set; }
	
	/// <summary>
	/// Single media file reference for this lesson
	/// </summary>
	public int? MediaFileId { get; set; }
	public string? MediaFileUrl { get; set; }

	/// <summary>
	/// True when the caller hasn't paid for this lesson, so no video location is returned.
	/// </summary>
	public bool IsLocked { get; set; }


}

public class LessonSearchDto
{
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 10;
	public string? SearchTerm { get; set; }
	public int? CourseId { get; set; }
	public bool? IsFree { get; set; }
}

public class CreateLessonDto 
{
	/// <summary>
	/// Title of the lesson
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// Detailed description of the lesson content
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Whether this lesson is free or requires purchase
	/// </summary>
	public bool IsFree { get; set; }

	/// <summary>
	/// Course ID that this lesson belongs to
	/// </summary>
	public int CourseId { get; set; }

	/// <summary>
	/// Single media file reference for this lesson
	/// </summary>
	public IFormFile? MediaFile { get; set; }


}

public class UpdateLessonDto
{
    /// <summary>
    /// Title of the lesson
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Detailed description of the lesson content
    /// </summary>
    public string? Description { get; set; }

	/// <summary>
	/// Whether this lesson is free or requires purchase
	/// </summary>
	public bool? IsFree { get; set; } = true;

    /// <summary>
    /// Course ID that this lesson belongs to
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// Single media file reference for this lesson
    /// </summary>
    public int? MediaFileId { get; set; }
    public IFormFile? NewMediaFile { get; set; }


}


