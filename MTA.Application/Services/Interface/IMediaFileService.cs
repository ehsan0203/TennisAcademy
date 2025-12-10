using Microsoft.AspNetCore.Http;
using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for MediaFile operations (with file storage)
/// </summary>
public interface IMediaFileService
{
    /// <summary>
    /// Get all media files with optional filtering (supports search, type, lesson, message)
    /// </summary>
    Task<PaginatedResult<MediaFileDto>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int? typeId = null,
        int? lessonId = null,
        int? messageId = null);

    /// <summary>
    /// Get media file by ID
    /// </summary>
    Task<MediaFileDto?> GetByIdAsync(int id);

    /// <summary>
    /// Get all GIF media files (image/gif) for reuse
    /// </summary>
    Task<IEnumerable<MediaFileDto>> GetGifsAsync();

    /// <summary>
    /// Create new media file (handles file upload)
    /// </summary>
    Task<MediaFileDto> CreateAsync(IFormFile file, MediaFileUploadDto mediaFileDto);

    /// <summary>
    /// Update existing media file (handles file replacement)
    /// </summary>
    Task<MediaFileDto> UpdateAsync(int id, IFormFile? file, MediaFileUploadDto mediaFileDto);

    /// <summary>
    /// Delete media file (removes file from storage)
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
