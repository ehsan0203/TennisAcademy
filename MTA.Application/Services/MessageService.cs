using AutoMapper;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MTA.Application.Services;

/// <summary>
/// Service implementation for Message operations
/// </summary>
public class MessageService : IMessageService
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;

	public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}

	/// <summary>
	/// Get all messages with optional filtering
	/// </summary>
	public async Task<PaginatedResult<MessageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? ticketId = null, int? senderId = null, bool? isRead = null)
	{
        var query = _unitOfWork.Repository<Message>().GetQueryable()
            .Include(m => m.MediaFile)
            .Include(m => m.Ticket)
            .Include(m => m.Sender)
                .ThenInclude(s => s.UserProfile).AsQueryable();

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

	/// <summary>
	/// Get message by ID
	/// </summary>
	public async Task<MessageDto?> GetByIdAsync(int id)
	{
		var message = await _unitOfWork.Repository<Message>().GetQueryable()
			.Include(m => m.MediaFile)
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
			.Include(m => m.MediaFile)
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
			.Include(m => m.MediaFile)
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
			.Include(m => m.MediaFile)
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
			.Include(m => m.MediaFile)
			.Where(m => m.TicketId == ticketId && !m.IsRead)
			.ToListAsync();
		return messages.Select(m => _mapper.Map<MessageDto>(m));
	}

	/// <summary>
	/// Create new message
	/// </summary>
	public async Task<MessageDto> CreateAsync(CreateMessageDto messageDto)
	{
		var message = _mapper.Map<Message>(messageDto);

		//TODO Save File 
		var MediaFileId = 0;

        message.MediaFileId = MediaFileId;
		message.CreatedAt = DateTime.UtcNow;
		
		var createdMessage = await _unitOfWork.Repository<Message>().AddAsync(message);
		await _unitOfWork.SaveChangesAsync();
		
		return _mapper.Map<MessageDto>(createdMessage);
	}

	/// <summary>
	/// Update existing message
	/// </summary>
	public async Task<MessageDto> UpdateAsync(int id, UpdateMessageDto messageDto)
	{
		var existingMessage = await _unitOfWork.Repository<Message>().GetByIdAsync(id);
		if (existingMessage == null)
			throw new ArgumentException($"Message with ID {id} not found");

		var MediaFileId = 0;

        //TODO if NewFile Exist 
        //TODO Save new File 
        //TODO Delete old File 

        existingMessage.MediaFileId = MediaFileId==0 ? messageDto.MediaFileId : MediaFileId;

        // Update only allowed fields
        existingMessage.Text = messageDto.Text;
		existingMessage.UpdatedAt = DateTime.UtcNow;

		var updatedMessage = await _unitOfWork.Repository<Message>().UpdateAsync(existingMessage);
		await _unitOfWork.SaveChangesAsync();
		
		return _mapper.Map<MessageDto>(updatedMessage);
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
