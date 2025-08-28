namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for MediaFile entity
/// </summary>
public class MediaFileDto : BaseDto
{
    /// <summary>
    /// Title of the media file
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// URL to the media file
    /// </summary>
    public required string Url { get; set; }
    
    /// <summary>
    /// Type of the media file
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Type ID for this media file
    /// </summary>
    public int TypeId { get; set; }
    
    /// <summary>
    /// Type value (e.g., Video, Audio, Document, Image)
    /// </summary>
    public string? TypeValue { get; set; }
    
    /// <summary>
    /// Lesson ID that this media file belongs to (if any)
    /// </summary>
    public int? LessonId { get; set; }
    
    /// <summary>
    /// Lesson title
    /// </summary>
    public string? LessonTitle { get; set; }
    
    /// <summary>
    /// Message ID that this media file belongs to (if any)
    /// </summary>
    public int? MessageId { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// File extension
    /// </summary>
    public string? FileExtension { get; set; }
}
