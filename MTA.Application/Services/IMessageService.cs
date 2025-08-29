using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Service interface for Message operations
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Get all messages with optional filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for text content</param>
    /// <param name="ticketId">Filter by ticket ID</param>
    /// <param name="senderId">Filter by sender ID</param>
    /// <param name="isRead">Filter by read status</param>
    /// <returns>Paginated list of messages</returns>
    Task<PaginatedResult<MessageDto>> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? ticketId = null, int? senderId = null, bool? isRead = null);
    
    /// <summary>
    /// Get message by ID
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <returns>Message details</returns>
    Task<MessageDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Get messages by ticket ID
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <returns>List of messages</returns>
    Task<IEnumerable<MessageDto>> GetByTicketAsync(int ticketId);
    
    /// <summary>
    /// Get messages by sender ID
    /// </summary>
    /// <param name="senderId">Sender ID</param>
    /// <returns>List of messages</returns>
    Task<IEnumerable<MessageDto>> GetBySenderAsync(int senderId);
    
    /// <summary>
    /// Get unread messages
    /// </summary>
    /// <returns>List of unread messages</returns>
    Task<IEnumerable<MessageDto>> GetUnreadMessagesAsync();
    
    /// <summary>
    /// Get unread messages by ticket ID
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <returns>List of unread messages</returns>
    Task<IEnumerable<MessageDto>> GetUnreadMessagesByTicketAsync(int ticketId);
    
    /// <summary>
    /// Create new message
    /// </summary>
    /// <param name="messageDto">Message data</param>
    /// <returns>Created message</returns>
    Task<MessageDto> CreateAsync(MessageDto messageDto);
    
    /// <summary>
    /// Update existing message
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <param name="messageDto">Updated message data</param>
    /// <returns>Updated message</returns>
    Task<MessageDto> UpdateAsync(int id, MessageDto messageDto);
    
    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Mark message as read
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <returns>Updated message</returns>
    Task<MessageDto> MarkAsReadAsync(int id);
    
    /// <summary>
    /// Mark message as unread
    /// </summary>
    /// <param name="id">Message ID</param>
    /// <returns>Updated message</returns>
    Task<MessageDto> MarkAsUnreadAsync(int id);
    
    /// <summary>
    /// Mark all messages in ticket as read
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <returns>Number of messages marked as read</returns>
    Task<int> MarkAllAsReadByTicketAsync(int ticketId);
       
    /// <summary>
    /// Get messages by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of messages</returns>
    Task<IEnumerable<MessageDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Message statistics DTO
/// </summary>
public class MessageStatisticsDto
{
    public int TotalMessages { get; set; }
    public int ReadMessages { get; set; }
    public int UnreadMessages { get; set; }
    public int MessagesWithMedia { get; set; }
    public double AverageMediaFilesPerMessage { get; set; }
    public int MessagesThisMonth { get; set; }
    public int MessagesLastMonth { get; set; }
    public double AverageResponseTime { get; set; } 
}
