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


    public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IMediaFileService mediaFileService, ILogger<MediaFileService> logger)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_mediaFileService = mediaFileService;
		_logger = logger;
	}

	/// <summary>
	/// Get all messages with optional filtering
	/// </summary>
	public async Task<PaginatedResult<MessageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? ticketId = null, int? senderId = null, bool? isRead = null)
	{
        var query = _unitOfWork.Repository<Message>().GetQueryable()
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
		var totalCount = await query.CountAsync();

		// Apply pagination
		var messages = await query
			.OrderByDescending(m => m.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

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


    public async Task<List<MediaFileDto>> GetMessageFilesAsync(int messageId)
    {
        // پیام و فایل‌های مربوطه را لود می‌کنیم
        var message = await _unitOfWork.Repository<Message>().GetQueryable()
            .Include(m => m.MediaFiles)
                .ThenInclude(mm => mm.MediaFile)
            .FirstOrDefaultAsync(m => m.Id == messageId);

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
    public async Task<MessageDto?> GetByIdAsync(int id)
	{
        var message = await _unitOfWork.Repository<Message>().GetQueryable()
            .Include(m => m.MediaFiles)             
                .ThenInclude(mm => mm.MediaFile)  
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(a => a.UserProfile)
            .FirstOrDefaultAsync(m => m.Id == id);

        return message != null ? _mapper.Map<MessageDto>(message) : null;
	}

	/// <summary>
	/// Get messages by ticket ID
	/// </summary>
	public async Task<IEnumerable<MessageDto>> GetByTicketAsync(int ticketId)
	{
        var messages = await _unitOfWork.Repository<Message>().GetQueryable()
            .Include(m => m.MediaFiles)              
                .ThenInclude(mm => mm.MediaFile)    
            .Where(m => m.TicketId == ticketId)
            .ToListAsync();

        return messages.Select(m => _mapper.Map<MessageDto>(m));
	}

    /// <summary>
    /// Get messages by sender ID
    /// </summary>
    public async Task<IEnumerable<MessageDto>> GetBySenderAsync(int senderId)
    {
        var messages = await _unitOfWork.Repository<Message>().GetQueryable()
            .Include(m => m.MediaFiles)              
                .ThenInclude(mm => mm.MediaFile)    
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(s => s.UserProfile)
            .Where(m => m.SenderId == senderId)
            .ToListAsync();

        return messages.Select(m => _mapper.Map<MessageDto>(m));
    }


    /// <summary>
    /// Get unread messages
    /// </summary>
    public async Task<IEnumerable<MessageDto>> GetUnreadMessagesAsync()
    {
        var messages = await _unitOfWork.Repository<Message>().GetQueryable()
            .Include(m => m.MediaFiles)               
                .ThenInclude(mm => mm.MediaFile)    
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(s => s.UserProfile)
            .Where(m => !m.IsRead)
            .ToListAsync();

        return messages.Select(m => _mapper.Map<MessageDto>(m));
    }


    /// <summary>
    /// Get unread messages by ticket ID
    /// </summary>
    public async Task<IEnumerable<MessageDto>> GetUnreadMessagesByTicketAsync(int ticketId)
    {
        var messages = await _unitOfWork.Repository<Message>().GetQueryable()
            .Include(m => m.MediaFiles)             
                .ThenInclude(mm => mm.MediaFile)   
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(s => s.UserProfile)
            .Where(m => m.TicketId == ticketId && !m.IsRead)
            .ToListAsync();

        return messages.Select(m => _mapper.Map<MessageDto>(m));
    }


    /// <summary>
    /// Create new message
    /// </summary>
    public async Task<MessageDto> CreateAsync(CreateMessageDto messageDto)
    {
        try
        {
            var message = _mapper.Map<Message>(messageDto);

            // -------------------------------
            // ذخیره فایل اگر وجود دارد
            // -------------------------------
            if (messageDto.MediaFiles != null && messageDto.MediaFiles.Any())
            {
                foreach (var file in messageDto.MediaFiles)
                {
                    var mediaFileDto = new MediaFileUploadDto
                    {
                        MediaType = "Message",
                        PlacementName = "Attachment",
                        Title = $"Message {messageDto.TicketId}"
                    };
                    var mediaFile = await _mediaFileService.CreateAsync(file, mediaFileDto);
                    message.MediaFiles.Add(new MessageMediaFile { MediaFileId = mediaFile.Id });
                }
            }

            message.CreatedAt = DateTime.UtcNow;

            var createdMessage = await _unitOfWork.Repository<Message>().AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return new MessageDto
            {
                Id = createdMessage.Id,
                Text = createdMessage.Text,
                IsRead = createdMessage.IsRead,
                TicketId = createdMessage.TicketId,
                SenderId = createdMessage.SenderId,
                MediaFiles = createdMessage.MediaFiles.Select(mf => new MediaFileDto
                {
                    Id = mf.MediaFile.Id,
                    Url = mf.MediaFile.Url
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating message for TicketId: {TicketId}", messageDto.TicketId);
            throw;
        }
    }

    public async Task<MessageDto> UpdateAsync(int id, UpdateMessageDto messageDto)
    {
        try
        {
            var existingMessage = await _unitOfWork.Repository<Message>()
			.GetQueryable()
			.Include(m => m.MediaFiles)
				.ThenInclude(mf => mf.MediaFile)
			.FirstOrDefaultAsync(m => m.Id == id);

            if (existingMessage == null)
                throw new ArgumentException($"Message with ID {id} not found");

            // -------------------------------
            // آپدیت یا ایجاد MediaFile
            // -------------------------------
            if (messageDto.NewMediaFiles != null && messageDto.NewMediaFiles.Any())
            {
                // حذف تمام MediaFile های قبلی
                if (existingMessage.MediaFiles != null && existingMessage.MediaFiles.Any())
                {
                    _unitOfWork.Repository<MessageMediaFile>().DeleteRangeAsync(existingMessage.MediaFiles);
                    existingMessage.MediaFiles.Clear();
                }

                foreach (var file in messageDto.NewMediaFiles)
                {
                    var mediaFileDto = new MediaFileUploadDto
                    {
                        MediaType = "Message",
                        PlacementName = "Attachment",
                        Title = $"Message {messageDto.TicketId}"
                    };

                    // ایجاد فایل جدید
                    var mediaFile = await _mediaFileService.CreateAsync(file, mediaFileDto);

                    // اضافه کردن به کالکشن پیام
                    existingMessage.MediaFiles.Add(new MessageMediaFile
                    {
                        MediaFileId = mediaFile.Id
                    });
                }
            }

            // -------------------------------
            // آپدیت پراپرتی‌های ساده
            // -------------------------------
            existingMessage.Text = messageDto.Text;
            existingMessage.IsRead = messageDto.IsRead;
            existingMessage.TicketId = messageDto.TicketId;
            existingMessage.SenderId = messageDto.SenderId;
            existingMessage.UpdatedAt = DateTime.UtcNow;

            // ذخیره تغییرات
            var updatedMessage = await _unitOfWork.Repository<Message>().UpdateAsync(existingMessage);
            await _unitOfWork.SaveChangesAsync();

            return new MessageDto
            {
                Id = updatedMessage.Id,
                Text = updatedMessage.Text,
                IsRead = updatedMessage.IsRead,
                TicketId = updatedMessage.TicketId,
                SenderId = updatedMessage.SenderId,
                MediaFiles = updatedMessage.MediaFiles.Select(mf => new MediaFileDto
                {
                    Id = mf.MediaFile.Id,
                    Url = mf.MediaFile.Url
                }).ToList()
            };
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
    public async Task<bool> DeleteAsync(int id)
	{
		var message = await _unitOfWork.Repository<Message>().GetByIdAsync(id);
		if (message == null)
			return false;

		await _unitOfWork.Repository<Message>().DeleteAsync(message.Id);
		await _unitOfWork.SaveChangesAsync();
		
		return true;
	}

	/// <summary>
	/// Mark message as read
	/// </summary>
	public async Task<MessageDto> MarkAsReadAsync(int id)
	{
		var message = await _unitOfWork.Repository<Message>().GetByIdAsync(id);
		if (message == null)
			throw new ArgumentException($"Message with ID {id} not found");

		message.IsRead = true;
		message.UpdatedAt = DateTime.UtcNow;

		var updatedMessage = await _unitOfWork.Repository<Message>().UpdateAsync(message);
		await _unitOfWork.SaveChangesAsync();
		
		return _mapper.Map<MessageDto>(updatedMessage);
	}

	/// <summary>
	/// Mark message as unread
	/// </summary>
	public async Task<MessageDto> MarkAsUnreadAsync(int id)
	{
		var message = await _unitOfWork.Repository<Message>().GetByIdAsync(id);
		if (message == null)
			throw new ArgumentException($"Message with ID {id} not found");

		message.IsRead = false;
		message.UpdatedAt = DateTime.UtcNow;

		var updatedMessage = await _unitOfWork.Repository<Message>().UpdateAsync(message);
		await _unitOfWork.SaveChangesAsync();
		
		return _mapper.Map<MessageDto>(updatedMessage);
	}

	/// <summary>
	/// Mark all messages in ticket as read
	/// </summary>
	public async Task<int> MarkAllAsReadByTicketAsync(int ticketId)
	{
		var messages = await _unitOfWork.Repository<Message>().GetAllAsync(m => m.TicketId == ticketId && !m.IsRead);
		
		foreach (var message in messages)
		{
			message.IsRead = true;
			message.UpdatedAt = DateTime.UtcNow;
			await _unitOfWork.Repository<Message>().UpdateAsync(message);
		}
		
		await _unitOfWork.SaveChangesAsync();
		return messages.Count();
	}

	/// <summary>
	/// Get messages by date range
	/// </summary>
	public async Task<IEnumerable<MessageDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
	{
		var messages = await _unitOfWork.Repository<Message>().GetAllAsync(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate);
		return messages.Select(m => _mapper.Map<MessageDto>(m));
	}
}
