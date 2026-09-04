using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Message operations
/// </summary>
public class MessageService : IMessageService
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;
    private readonly IMediaFileService _mediaFileService;
    private readonly ILogger<MediaFileService> _logger;
    private readonly IChatNotifier _chatNotifier;


    public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IMediaFileService mediaFileService, ILogger<MediaFileService> logger, IChatNotifier chatNotifier)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_mediaFileService = mediaFileService;
		_logger = logger;
		_chatNotifier = chatNotifier;
	}

	/// <summary>
	/// Get all messages with optional filtering
	/// </summary>
	public async Task<PaginatedResult<MessageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? ticketId = null, int? senderId = null, bool? isRead = null, CancellationToken ct = default)
	{
        var query = _unitOfWork.Repository<Message>().GetQueryable()
            .AsNoTracking()
            .Include(m => m.MediaFiles)
                .ThenInclude(mm => mm.MediaFile)
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(s => s.UserProfile)
            .AsQueryable();


        if (!string.IsNullOrEmpty(searchTerm))
		{
			query = query.Where(m => m.Text.Contains(searchTerm));
		}

		if (ticketId.HasValue)
		{
			query = query.Where(m => m.TicketId == ticketId.Value);
		}

		if (senderId.HasValue)
		{
			query = query.Where(m => m.SenderId == senderId.Value);
		}

		if (isRead.HasValue)
		{
			query = query.Where(m => m.IsRead == isRead.Value);
		}

		// Get total count
		var totalCount = await query.CountAsync(ct);

		// Apply pagination
		var messages = await query
			.OrderByDescending(m => m.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(ct);

		// Map to DTOs with additional data
		var messageDtos = messages.Select(m => _mapper.Map<MessageDto>(m)).ToList();

		return new PaginatedResult<MessageDto>
		{
			Data = messageDtos,
			TotalCount = totalCount,
			Page = page,
			PageSize = pageSize,
		};
	}


    public async Task<List<MediaFileDto>> GetMessageFilesAsync(int messageId, CancellationToken ct = default)
    {
        // پیام و فایل‌های مربوطه را لود می‌کنیم
        var message = await _unitOfWork.Repository<Message>().GetQueryable()
            .AsNoTracking()
            .Include(m => m.MediaFiles)
                .ThenInclude(mm => mm.MediaFile)
            .FirstOrDefaultAsync(m => m.Id == messageId, ct);

        if (message == null)
            throw new ArgumentException($"Message with ID {messageId} not found");

        // همه فایل‌ها را به MediaFileDto مپ می‌کنیم
        var mediaFiles = message.MediaFiles
            .Select(mm => new MediaFileDto
            {
                Id = mm.MediaFile.Id,
                Url = mm.MediaFile.Url,
                Title = mm.MediaFile.Title,
                MediaType = mm.MediaFile.Type.ToString(),
                CreatedAt = mm.MediaFile.CreatedAt
            })
            .ToList();

        return mediaFiles;
    }


    /// <summary>
    /// Get message by ID
    /// </summary>
    public async Task<MessageDto?> GetByIdAsync(int id, CancellationToken ct = default)
	{
        var message = await _unitOfWork.Repository<Message>().GetQueryable()
            .AsNoTracking()
            .Include(m => m.MediaFiles)
                .ThenInclude(mm => mm.MediaFile)
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(a => a.UserProfile)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

        return message != null ? _mapper.Map<MessageDto>(message) : null;
	}

	/// <summary>
	/// Get messages by ticket ID
	/// </summary>
	public async Task<IEnumerable<MessageDto>> GetByTicketAsync(int ticketId, CancellationToken ct = default)
	{
        var messages = await _unitOfWork.Repository<Message>().GetQueryable()
            .AsNoTracking()
            .Include(m => m.MediaFiles)
                .ThenInclude(mm => mm.MediaFile)
            .Where(m => m.TicketId == ticketId)
            .ToListAsync(ct);

        return messages.Select(m => _mapper.Map<MessageDto>(m));
	}

    /// <summary>
    /// Get messages by sender ID
    /// </summary>
    public async Task<IEnumerable<MessageDto>> GetBySenderAsync(int senderId, CancellationToken ct = default)
    {
        var messages = await _unitOfWork.Repository<Message>().GetQueryable()
            .AsNoTracking()
            .Include(m => m.MediaFiles)
                .ThenInclude(mm => mm.MediaFile)
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(s => s.UserProfile)
            .Where(m => m.SenderId == senderId)
            .ToListAsync(ct);

        return messages.Select(m => _mapper.Map<MessageDto>(m));
    }


    /// <summary>
    /// Get unread messages
    /// </summary>
    public async Task<IEnumerable<MessageDto>> GetUnreadMessagesAsync(CancellationToken ct = default)
    {
        var messages = await _unitOfWork.Repository<Message>().GetQueryable()
            .AsNoTracking()
            .Include(m => m.MediaFiles)
                .ThenInclude(mm => mm.MediaFile)
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(s => s.UserProfile)
            .Where(m => !m.IsRead)
            .ToListAsync(ct);

        return messages.Select(m => _mapper.Map<MessageDto>(m));
    }


    /// <summary>
    /// Get unread messages by ticket ID
    /// </summary>
    public async Task<IEnumerable<MessageDto>> GetUnreadMessagesByTicketAsync(int ticketId, CancellationToken ct = default)
    {
        var messages = await _unitOfWork.Repository<Message>().GetQueryable()
            .AsNoTracking()
            .Include(m => m.MediaFiles)
                .ThenInclude(mm => mm.MediaFile)
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(s => s.UserProfile)
            .Where(m => m.TicketId == ticketId && !m.IsRead)
            .ToListAsync(ct);

        return messages.Select(m => _mapper.Map<MessageDto>(m));
    }


    /// <summary>
    /// Create new message
    /// </summary>
    public async Task<MessageDto> CreateAsync(CreateMessageDto messageDto, CancellationToken ct = default)
    {
        try
        {
            var message = _mapper.Map<Message>(messageDto);

            // Link any existing media files (e.g., GIFs) without re-uploading
            if (messageDto.MediaFileIds != null && messageDto.MediaFileIds.Any())
            {
                var distinctIds = messageDto.MediaFileIds.Distinct().ToList();
                var existingIds = await _unitOfWork.Repository<MediaFile>().GetQueryable()
                    .Where(mf => distinctIds.Contains(mf.Id))
                    .Select(mf => mf.Id)
                    .ToListAsync(ct);

                var missingIds = distinctIds.Except(existingIds).ToList();
                if (missingIds.Any())
                    throw new ArgumentException($"Media files not found for IDs: {string.Join(", ", missingIds)}");

                foreach (var mediaId in distinctIds)
                {
                    message.MediaFiles.Add(new MessageMediaFile { MediaFileId = mediaId });
                }
            }

            // Upload new attachments when provided
            if (messageDto.MediaFiles != null && messageDto.MediaFiles.Any())
            {
                foreach (var file in messageDto.MediaFiles)
                {
                    var mediaFileDto = new MediaFileUploadDto
                    {
                        MediaType = InferMediaType(file),
                        PlacementName = null,
                        Title = $"Message {messageDto.TicketId}"
                    };
                    var mediaFile = await _mediaFileService.CreateAsync(file, mediaFileDto);
                    message.MediaFiles.Add(new MessageMediaFile { MediaFileId = mediaFile.Id });
                }
            }

            var createdMessage = await _unitOfWork.Repository<Message>().AddAsync(message, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var createdWithIncludes = await _unitOfWork.Repository<Message>().GetQueryable()
                .Include(m => m.MediaFiles)
                    .ThenInclude(mm => mm.MediaFile)
                .Include(m => m.Ticket)
                .Include(m => m.Sender)
                    .ThenInclude(s => s.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == createdMessage.Id, ct);

            var resultDto = createdWithIncludes != null
                ? _mapper.Map<MessageDto>(createdWithIncludes)
                : _mapper.Map<MessageDto>(createdMessage);

            await _chatNotifier.NotifyMessageCreatedAsync(messageDto.TicketId, resultDto, ct);
            return resultDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating message for TicketId: {TicketId}", messageDto.TicketId);
            throw;
        }
    }

    public async Task<MessageDto> UpdateAsync(int id, UpdateMessageDto messageDto, CancellationToken ct = default)
    {
        try
        {
            var existingMessage = await _unitOfWork.Repository<Message>()
                .GetQueryable()
                .Include(m => m.MediaFiles)
                    .ThenInclude(mf => mf.MediaFile)
                .FirstOrDefaultAsync(m => m.Id == id, ct);

            if (existingMessage == null)
                throw new ArgumentException($"Message with ID {id} not found");

            var hasNewMediaFiles = messageDto.NewMediaFiles != null && messageDto.NewMediaFiles.Any();
            var hasExistingMediaIds = messageDto.MediaFileIds != null && messageDto.MediaFileIds.Any();

            if (hasNewMediaFiles || hasExistingMediaIds)
            {
                if (existingMessage.MediaFiles != null && existingMessage.MediaFiles.Any())
                {
                    await _unitOfWork.Repository<MessageMediaFile>().DeleteRangeAsync(existingMessage.MediaFiles);
                    existingMessage.MediaFiles.Clear();
                }

                if (hasExistingMediaIds)
                {
                    var distinctIds = messageDto.MediaFileIds!.Distinct().ToList();
                    var existingIds = await _unitOfWork.Repository<MediaFile>().GetQueryable()
                        .Where(mf => distinctIds.Contains(mf.Id))
                        .Select(mf => mf.Id)
                        .ToListAsync(ct);

                    var missingIds = distinctIds.Except(existingIds).ToList();
                    if (missingIds.Any())
                        throw new ArgumentException($"Media files not found for IDs: {string.Join(", ", missingIds)}");

                    foreach (var mediaId in distinctIds)
                    {
                        existingMessage.MediaFiles.Add(new MessageMediaFile
                        {
                            MediaFileId = mediaId
                        });
                    }
                }

                if (hasNewMediaFiles)
                {
                    foreach (var file in messageDto.NewMediaFiles!)
                    {
                        var mediaFileDto = new MediaFileUploadDto
                        {
                            MediaType = InferMediaType(file),
                            PlacementName = null,
                            Title = $"Message {messageDto.TicketId}"
                        };

                        var mediaFile = await _mediaFileService.CreateAsync(file, mediaFileDto);

                        existingMessage.MediaFiles.Add(new MessageMediaFile
                        {
                            MediaFileId = mediaFile.Id
                        });
                    }
                }
            }

            existingMessage.Text = messageDto.Text;
            existingMessage.IsRead = messageDto.IsRead;
            existingMessage.TicketId = messageDto.TicketId;
            existingMessage.SenderId = messageDto.SenderId;

            await _unitOfWork.Repository<Message>().UpdateAsync(existingMessage, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var updatedWithIncludes = await _unitOfWork.Repository<Message>().GetQueryable()
                .Include(m => m.MediaFiles)
                    .ThenInclude(mm => mm.MediaFile)
                .Include(m => m.Ticket)
                .Include(m => m.Sender)
                    .ThenInclude(s => s.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == existingMessage.Id, ct);

            return updatedWithIncludes != null
                ? _mapper.Map<MessageDto>(updatedWithIncludes)
                : _mapper.Map<MessageDto>(existingMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating message with ID: {MessageId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete message
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
	{
		var message = await _unitOfWork.Repository<Message>().GetQueryable()
			.Include(m => m.MediaFiles)
			.FirstOrDefaultAsync(m => m.Id == id, ct);
		if (message == null)
			return false;

		var mediaFileIds = message.MediaFiles.Select(mm => mm.MediaFileId).ToList();

		await _unitOfWork.Repository<Message>().DeleteAsync(message.Id, ct);
		await _unitOfWork.SaveChangesAsync(ct);

		// Delete the underlying media files (and their physical files on disk) —
		// soft-deleting the message alone left these behind forever, leaking disk space.
		foreach (var mediaFileId in mediaFileIds)
		{
			await _mediaFileService.DeleteAsync(mediaFileId);
		}

		return true;
	}

	/// <summary>
	/// Mark message as read
	/// </summary>
	public async Task<MessageDto> MarkAsReadAsync(int id, CancellationToken ct = default)
	{
		var message = await _unitOfWork.Repository<Message>().GetByIdAsync(id, ct);
		if (message == null)
			throw new ArgumentException($"Message with ID {id} not found");

		message.IsRead = true;

		var updatedMessage = await _unitOfWork.Repository<Message>().UpdateAsync(message, ct);
		await _unitOfWork.SaveChangesAsync(ct);

		return _mapper.Map<MessageDto>(updatedMessage);
	}

	/// <summary>
	/// Mark message as unread
	/// </summary>
	public async Task<MessageDto> MarkAsUnreadAsync(int id, CancellationToken ct = default)
	{
		var message = await _unitOfWork.Repository<Message>().GetByIdAsync(id, ct);
		if (message == null)
			throw new ArgumentException($"Message with ID {id} not found");

		message.IsRead = false;

		var updatedMessage = await _unitOfWork.Repository<Message>().UpdateAsync(message, ct);
		await _unitOfWork.SaveChangesAsync(ct);

		return _mapper.Map<MessageDto>(updatedMessage);
	}

	/// <summary>
	/// Mark all messages in ticket as read
	/// </summary>
	public async Task<int> MarkAllAsReadByTicketAsync(int ticketId, CancellationToken ct = default)
	{
		var messages = await _unitOfWork.Repository<Message>().GetAllAsync(m => m.TicketId == ticketId && !m.IsRead, ct: ct);

		foreach (var message in messages)
		{
			message.IsRead = true;
			await _unitOfWork.Repository<Message>().UpdateAsync(message, ct);
		}

		await _unitOfWork.SaveChangesAsync(ct);
		return messages.Count();
	}

	/// <summary>
	/// Get messages by date range
	/// </summary>
	public async Task<IEnumerable<MessageDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
	{
		var messages = await _unitOfWork.Repository<Message>().GetAllAsync(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate, ct: ct);
		return messages.Select(m => _mapper.Map<MessageDto>(m));
	}

	private static string InferMediaType(Microsoft.AspNetCore.Http.IFormFile file)
	{
		var ct = (file.ContentType ?? string.Empty).ToLowerInvariant();
		var ext = System.IO.Path.GetExtension(file.FileName ?? string.Empty).ToLowerInvariant();

		if (ct.StartsWith("image/") || ext is ".gif" or ".png" or ".jpg" or ".jpeg" or ".webp" or ".bmp" or ".svg")
			return ct.Contains("gif") || ext == ".gif" ? "Video" : "Image";
		if (ct.StartsWith("video/") || ext is ".mp4" or ".mov" or ".webm" or ".mkv" or ".avi")
			return "Video";
		if (ct.StartsWith("audio/") || ext is ".mp3" or ".wav" or ".ogg" or ".m4a")
			return "Audio";
		return "Document";
	}
}
