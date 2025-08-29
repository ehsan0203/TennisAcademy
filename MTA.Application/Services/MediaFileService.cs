using AutoMapper;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for MediaFile operations
/// </summary>
public class MediaFileService : IMediaFileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MediaFileService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all media files with optional filtering
    /// </summary>
    public async Task<PaginatedResult<MediaFileDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? typeId = null, int? lessonId = null, int? messageId = null)
    {
        var query = _unitOfWork.Repository<MediaFile>().GetQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(m => m.Title.Contains(searchTerm));
        }

        if (typeId.HasValue)
        {
            query = query.Where(m => m.TypeId == typeId.Value);
        }

        if (lessonId.HasValue)
        {
            query = query.Where(m => m.LessonId == lessonId.Value);
        }

        if (messageId.HasValue)
        {
            query = query.Where(m => m.MessageId == messageId.Value);
        }

        // Get total count
        var totalCount = await _unitOfWork.Repository<MediaFile>().CountAsync(query);

        // Apply pagination
        var mediaFiles = await _unitOfWork.Repository<MediaFile>().GetPagedAsync(query, page, pageSize);

        // Map to DTOs with additional data
        var mediaFileDtos = mediaFiles.Select(m => _mapper.Map<MediaFileDto>(m)).ToList();

        return new PaginatedResult<MediaFileDto>
        {
            Data = mediaFileDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    /// <summary>
    /// Get media file by ID
    /// </summary>
    public async Task<MediaFileDto?> GetByIdAsync(int id)
    {
        var mediaFile = await _unitOfWork.Repository<MediaFile>().GetByIdAsync(id);
        return mediaFile != null ? _mapper.Map<MediaFileDto>(mediaFile) : null;
    }

    /// <summary>
    /// Get media files by type ID
    /// </summary>
    public async Task<IEnumerable<MediaFileDto>> GetByTypeAsync(int typeId)
    {
        var mediaFiles = await _unitOfWork.Repository<MediaFile>().GetAllAsync(m => m.TypeId == typeId);
        return mediaFiles.Select(m => _mapper.Map<MediaFileDto>(m));
    }

    /// <summary>
    /// Get media files by lesson ID
    /// </summary>
    public async Task<IEnumerable<MediaFileDto>> GetByLessonAsync(int lessonId)
    {
        var mediaFiles = await _unitOfWork.Repository<MediaFile>().GetAllAsync(m => m.LessonId == lessonId);
        return mediaFiles.Select(m => _mapper.Map<MediaFileDto>(m));
    }

    /// <summary>
    /// Get media files by message ID
    /// </summary>
    public async Task<IEnumerable<MediaFileDto>> GetByMessageAsync(int messageId)
    {
        var mediaFiles = await _unitOfWork.Repository<MediaFile>().GetAllAsync(m => m.MessageId == messageId);
        return mediaFiles.Select(m => _mapper.Map<MediaFileDto>(m));
    }

    /// <summary>
    /// Create new media file
    /// </summary>
    public async Task<MediaFileDto> CreateAsync(MediaFileDto mediaFileDto)
    {
        var mediaFile = _mapper.Map<MediaFile>(mediaFileDto);
        mediaFile.CreatedAt = DateTime.UtcNow;
        
        var createdMediaFile = await _unitOfWork.Repository<MediaFile>().AddAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<MediaFileDto>(createdMediaFile);
    }

    /// <summary>
    /// Update existing media file
    /// </summary>
    public async Task<MediaFileDto> UpdateAsync(int id, MediaFileDto mediaFileDto)
    {
        var existingMediaFile = await _unitOfWork.Repository<MediaFile>().GetByIdAsync(id);
        if (existingMediaFile == null)
            throw new ArgumentException($"Media file with ID {id} not found");

        // Update only allowed fields
        existingMediaFile.Title = mediaFileDto.Title;
        existingMediaFile.Url = mediaFileDto.Url;
        existingMediaFile.TypeId = mediaFileDto.TypeId;
        existingMediaFile.LessonId = mediaFileDto.LessonId;
        existingMediaFile.MessageId = mediaFileDto.MessageId;
        existingMediaFile.UpdatedAt = DateTime.UtcNow;

        var updatedMediaFile = await _unitOfWork.Repository<MediaFile>().UpdateAsync(existingMediaFile);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<MediaFileDto>(updatedMediaFile);
    }

    /// <summary>
    /// Delete media file
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var mediaFile = await _unitOfWork.Repository<MediaFile>().GetByIdAsync(id);
        if (mediaFile == null)
            return false;

        await _unitOfWork.Repository<MediaFile>().DeleteAsync(mediaFile.Id);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Update media file type
    /// </summary>
    public async Task<MediaFileDto> UpdateTypeAsync(int id, int typeId)
    {
        var mediaFile = await _unitOfWork.Repository<MediaFile>().GetByIdAsync(id);
        if (mediaFile == null)
            throw new ArgumentException($"Media file with ID {id} not found");

        mediaFile.TypeId = typeId;
        mediaFile.UpdatedAt = DateTime.UtcNow;

        var updatedMediaFile = await _unitOfWork.Repository<MediaFile>().UpdateAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<MediaFileDto>(updatedMediaFile);
    }

    /// <summary>
    /// Update media file URL
    /// </summary>
    public async Task<MediaFileDto> UpdateUrlAsync(int id, string url)
    {
        var mediaFile = await _unitOfWork.Repository<MediaFile>().GetByIdAsync(id);
        if (mediaFile == null)
            throw new ArgumentException($"Media file with ID {id} not found");

        mediaFile.Url = url;
        mediaFile.UpdatedAt = DateTime.UtcNow;

        var updatedMediaFile = await _unitOfWork.Repository<MediaFile>().UpdateAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<MediaFileDto>(updatedMediaFile);
    }

    /// <summary>
    /// Get media files by date range
    /// </summary>
    public async Task<IEnumerable<MediaFileDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var mediaFiles = await _unitOfWork.Repository<MediaFile>().GetAllAsync(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate);
        return mediaFiles.Select(m => _mapper.Map<MediaFileDto>(m));
    }

    /// <summary>
    /// Get media files by file size range
    /// </summary>
    public async Task<IEnumerable<MediaFileDto>> GetByFileSizeRangeAsync(long minSize, long maxSize)
    {
        // This would need actual file size storage in the database
        // For now, returning empty list as placeholder
        var mediaFiles = await _unitOfWork.Repository<MediaFile>().GetAllAsync();
        return mediaFiles.Select(m => _mapper.Map<MediaFileDto>(m));
    }
}
