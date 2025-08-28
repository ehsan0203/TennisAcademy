using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for MediaFile operations
/// </summary>
public interface IMediaFileService
{
    /// <summary>
    /// Get all media files with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for title</param>
    /// <param name="typeId">Filter by type ID</param>
    /// <param name="lessonId">Filter by lesson ID</param>
    /// <param name="messageId">Filter by message ID</param>
    /// <returns>Paginated list of media files</returns>
    Task<PaginatedResult<MediaFileDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? typeId = null, int? lessonId = null, int? messageId = null);
    
    /// <summary>
    /// Get media file by ID
    /// </summary>
    /// <param name="id">Media file ID</param>
    /// <returns>Media file details</returns>
    Task<MediaFileDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get media files by type ID
    /// </summary>
    /// <param name="typeId">Type ID</param>
    /// <returns>List of media files</returns>
    Task<IEnumerable<MediaFileDto>> GetByTypeAsync(int typeId);
    
    /// <summary>
    /// Get media files by lesson ID
    /// </summary>
    /// <param name="lessonId">Lesson ID</param>
    /// <returns>List of media files</returns>
    Task<IEnumerable<MediaFileDto>> GetByLessonAsync(int lessonId);
    
    /// <summary>
    /// Get media files by message ID
    /// </summary>
    /// <param name="messageId">Message ID</param>
    /// <returns>List of media files</returns>
    Task<IEnumerable<MediaFileDto>> GetByMessageAsync(int messageId);
    
    /// <summary>
    /// Get media files by file extension
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>List of media files</returns>
    Task<IEnumerable<MediaFileDto>> GetByExtensionAsync(string extension);
    
    /// <summary>
    /// Create new media file
    /// </summary>
    /// <param name="mediaFileDto">Media file data</param>
    /// <returns>Created media file</returns>
    Task<MediaFileDto> CreateAsync(MediaFileDto mediaFileDto);
    
    /// <summary>
    /// Update existing media file
    /// </summary>
    /// <param name="id">Media file ID</param>
    /// <param name="mediaFileDto">Updated media file data</param>
    /// <returns>Updated media file</returns>
    Task<MediaFileDto> UpdateAsync(int id, MediaFileDto mediaFileDto);
    
    /// <summary>
    /// Delete media file
    /// </summary>
    /// <param name="id">Media file ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Update media file type
    /// </summary>
    /// <param name="id">Media file ID</param>
    /// <param name="typeId">New type ID</param>
    /// <returns>Updated media file</returns>
    Task<MediaFileDto> UpdateTypeAsync(int id, int typeId);
    
    /// <summary>
    /// Update media file URL
    /// </summary>
    /// <param name="id">Media file ID</param>
    /// <param name="url">New URL</param>
    /// <returns>Updated media file</returns>
    Task<MediaFileDto> UpdateUrlAsync(int id, string url);
    
    /// <summary>
    /// Get media file statistics
    /// </summary>
    /// <returns>Media file statistics</returns>
    Task<MediaFileStatisticsDto> GetStatisticsAsync();
    
    /// <summary>
    /// Get media files by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of media files</returns>
    Task<IEnumerable<MediaFileDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Get media files by file size range
    /// </summary>
    /// <param name="minSize">Minimum file size in bytes</param>
    /// <param name="maxSize">Maximum file size in bytes</param>
    /// <returns>List of media files</returns>
    Task<IEnumerable<MediaFileDto>> GetByFileSizeRangeAsync(long minSize, long maxSize);
}

/// <summary>
/// Media file statistics DTO
/// </summary>
public class MediaFileStatisticsDto
{
    public int TotalMediaFiles { get; set; }
    public long TotalFileSize { get; set; } // in bytes
    public double AverageFileSize { get; set; } // in bytes
    public int FilesByTypeVideo { get; set; }
    public int FilesByTypeAudio { get; set; }
    public int FilesByTypeDocument { get; set; }
    public int FilesByTypeImage { get; set; }
    public int FilesByTypeOther { get; set; }
    public int FilesInLessons { get; set; }
    public int FilesInMessages { get; set; }
    public int FilesThisMonth { get; set; }
    public int FilesLastMonth { get; set; }
    public Dictionary<string, int> FilesPerExtension { get; set; } = new();
}
