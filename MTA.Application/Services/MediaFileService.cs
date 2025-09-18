using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for MediaFile operations (handles file storage)
/// </summary>
public class MediaFileService : IMediaFileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<MediaFileService> _logger;

    public MediaFileService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileStorageService fileStorageService,
        ILogger<MediaFileService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<PaginatedResult<MediaFileDto>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int? typeId = null,
        int? lessonId = null,
        int? messageId = null)
    {
        IQueryable<MediaFile> query = _unitOfWork.Repository<MediaFile>().GetQueryable()
            .Include(m => m.Type)
            .Include(m => m.Lesson)
            .Include(m => m.Message);

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(m => m.Title.Contains(searchTerm));

        if (typeId.HasValue)
            query = query.Where(m => m.TypeId == typeId.Value);

        if (lessonId.HasValue)
            query = query.Where(m => m.LessonId == lessonId.Value);

        if (messageId.HasValue)
            query = query.Where(m => m.MessageId == messageId.Value);

        var totalCount = await query.CountAsync();
        var mediaFiles = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mediaFileDtos = mediaFiles.Select(m => _mapper.Map<MediaFileDto>(m)).ToList();

        return new PaginatedResult<MediaFileDto>
        {
            Data = mediaFileDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<MediaFileDto?> GetByIdAsync(int id)
    {
        var mediaFile = await _unitOfWork.Repository<MediaFile>().GetQueryable()
            .Include(m => m.Type)
            .Include(m => m.Placement)
            .Include(m => m.Lesson)
            .Include(m => m.Message)
            .FirstOrDefaultAsync(m => m.Id == id);

        return mediaFile != null ? _mapper.Map<MediaFileDto>(mediaFile) : null;
    }

    public async Task<MediaFileDto> CreateAsync(IFormFile file, MediaFileUploadDto mediaFileDto)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required");

        // گرفتن TypeId از جدول Lookup
        var typeEntity = await _unitOfWork.Repository<Lookup>()
            .GetQueryable()
            .FirstOrDefaultAsync(l => l.Category == "MediaType" && l.Key == mediaFileDto.MediaType);

        if (typeEntity == null)
            throw new ArgumentException($"Invalid MediaType: {mediaFileDto.MediaType}");

        // گرفتن PlacementId از جدول Lookup
        Lookup placementEntity = null;
        if (!string.IsNullOrEmpty(mediaFileDto.PlacementName))
        {
            placementEntity = await _unitOfWork.Repository<Lookup>()
                .GetQueryable()
                .FirstOrDefaultAsync(l => l.Category == "MediaPlacement" && l.Key == mediaFileDto.PlacementName);

            if (placementEntity == null)
                throw new ArgumentException($"Invalid MediaPlacement: {mediaFileDto.PlacementName}");
        }

        // گرفتن Lesson در صورت وجود
        Lesson lessonEntity = null;
        if (mediaFileDto.LessonId.HasValue)
        {
            lessonEntity = await _unitOfWork.Repository<Lesson>()
                .GetQueryable()
                .FirstOrDefaultAsync(l => l.Id == mediaFileDto.LessonId.Value);

            if (lessonEntity == null)
                throw new ArgumentException($"Invalid LessonId: {mediaFileDto.LessonId}");
        }

        // گرفتن Message در صورت وجود
        Message messageEntity = null;
        if (mediaFileDto.MessageId.HasValue)
        {
            messageEntity = await _unitOfWork.Repository<Message>()
                .GetQueryable()
                .FirstOrDefaultAsync(m => m.Id == mediaFileDto.MessageId.Value);

            if (messageEntity == null)
                throw new ArgumentException($"Invalid MessageId: {mediaFileDto.MessageId}");
        }

        // Map و پر کردن فیلدهای ضروری
        var mediaFile = _mapper.Map<MediaFile>(mediaFileDto);
        mediaFile.TypeId = typeEntity.Id;

        if (placementEntity != null)
        {
            mediaFile.PlacementId = placementEntity.Id;
        }

        if (lessonEntity != null)
        {
            mediaFile.LessonId = lessonEntity.Id;
        }

        if (messageEntity != null)
        {
            mediaFile.MessageId = messageEntity.Id;
        }

        mediaFile.Url = await _fileStorageService.SaveFileAsync(file, mediaFileDto.MediaType, mediaFileDto.PlacementName);
        mediaFile.FileSize = file.Length;
        mediaFile.FileExtension = Path.GetExtension(file.FileName);
        mediaFile.CreatedAt = DateTime.UtcNow;
        mediaFile.UpdatedAt = DateTime.UtcNow;

        var created = await _unitOfWork.Repository<MediaFile>().AddAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MediaFileDto>(created);
    }


    public async Task<MediaFileDto> UpdateAsync(int id, IFormFile? file, MediaFileUploadDto mediaFileDto)
    {
        var existing = await _unitOfWork.Repository<MediaFile>().GetByIdAsync(id);
        if (existing == null)
            throw new ArgumentException($"Media file with ID {id} not found");

        // -------------------------------
        // اعتبارسنجی MediaType
        // -------------------------------
        var typeEntity = await _unitOfWork.Repository<Lookup>()
            .GetQueryable()
            .FirstOrDefaultAsync(l => l.Category == "MediaType" && l.Key == mediaFileDto.MediaType);

        if (typeEntity == null)
            throw new ArgumentException($"Invalid MediaType: {mediaFileDto.MediaType}");

        // -------------------------------
        // اعتبارسنجی Placement
        // -------------------------------
        Lookup? placementEntity = null;
        if (!string.IsNullOrEmpty(mediaFileDto.PlacementName))
        {
            placementEntity = await _unitOfWork.Repository<Lookup>()
                .GetQueryable()
                .FirstOrDefaultAsync(l => l.Category == "MediaPlacement" && l.Key == mediaFileDto.PlacementName);

            if (placementEntity == null)
                throw new ArgumentException($"Invalid MediaPlacement: {mediaFileDto.PlacementName}");
        }

        // -------------------------------
        // اعتبارسنجی Lesson
        // -------------------------------
        Lesson? lessonEntity = null;
        if (mediaFileDto.LessonId.HasValue)
        {
            lessonEntity = await _unitOfWork.Repository<Lesson>()
                .GetQueryable()
                .FirstOrDefaultAsync(l => l.Id == mediaFileDto.LessonId.Value);

            if (lessonEntity == null)
                throw new ArgumentException($"Invalid LessonId: {mediaFileDto.LessonId}");
        }

        // -------------------------------
        // اعتبارسنجی Message
        // -------------------------------
        Message? messageEntity = null;
        if (mediaFileDto.MessageId.HasValue)
        {
            messageEntity = await _unitOfWork.Repository<Message>()
                .GetQueryable()
                .FirstOrDefaultAsync(m => m.Id == mediaFileDto.MessageId.Value);

            if (messageEntity == null)
                throw new ArgumentException($"Invalid MessageId: {mediaFileDto.MessageId}");
        }

        // -------------------------------
        // به‌روزرسانی فایل در صورت ارسال
        // -------------------------------
        if (file != null && file.Length > 0)
        {
            if (!string.IsNullOrEmpty(existing.Url))
                await _fileStorageService.DeleteFileAsync(existing.Url);

            var relativePath = await _fileStorageService.SaveFileAsync(file, mediaFileDto.MediaType, mediaFileDto.PlacementName);
            existing.Url = relativePath;
            existing.FileSize = file.Length;
            existing.FileExtension = Path.GetExtension(file.FileName);
        }

        // -------------------------------
        // به‌روزرسانی فیلدها
        // -------------------------------
        existing.Title = mediaFileDto.Title;
        existing.TypeId = typeEntity.Id;
        existing.PlacementId = placementEntity?.Id;
        existing.LessonId = lessonEntity?.Id;
        existing.MessageId = messageEntity?.Id;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _unitOfWork.Repository<MediaFile>().UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MediaFileDto>(updated);
    }


    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _unitOfWork.Repository<MediaFile>().GetByIdAsync(id);
        if (existing == null)
            return false;

        if (!string.IsNullOrEmpty(existing.Url))
            await _fileStorageService.DeleteFileAsync(existing.Url);

        await _unitOfWork.Repository<MediaFile>().DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
