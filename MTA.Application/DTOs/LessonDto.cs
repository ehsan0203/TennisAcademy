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
    /// Course title
    /// </summary>
    public string? CourseTitle { get; set; }
    
    /// <summary>
    /// Number of media files in this lesson
    /// </summary>
    public int MediaFileCount { get; set; }
    
    /// <summary>
    /// Order of this lesson within the course
    /// </summary>
    public int Order { get; set; }
    public bool CanDownload { get; set; } = false;
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
    /// Course title
    /// </summary>
    public string? CourseTitle { get; set; }

    /// <summary>
    /// Number of media files in this lesson
    /// </summary>
    public int MediaFileCount { get; set; }

    /// <summary>
    /// Order of this lesson within the course
    /// </summary>
    public int Order { get; set; }
    public bool CanDownload { get; set; } = false;
}


