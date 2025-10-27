using Microsoft.AspNetCore.Http;

namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Message entity
/// </summary>
public class MessageDto : BaseDto
{
	/// <summary>
	/// Text content of the message
	/// </summary>
	public required string Text { get; set; }
	
	/// <summary>
	/// Whether the message has been read
	/// </summary>
	public bool IsRead { get; set; }
	
	/// <summary>
	/// Ticket ID that this message belongs to
	/// </summary>
	public int TicketId { get; set; }
	
	/// <summary>
	/// Ticket topic
	/// </summary>
	public string? TicketTopic { get; set; }
	
	/// <summary>
	/// Sender ID of the message
	/// </summary>
	public int SenderId { get; set; }
	
	/// <summary>
	/// Sender's first name
	/// </summary>
	public string? SenderFirstName { get; set; }
	
	/// <summary>
	/// Sender's last name
	/// </summary>
	public string? SenderLastName { get; set; }
	
	/// <summary>
	/// Sender's profile image
	/// </summary>
	public string? SenderImage { get; set; }
    public List<MediaFileDto>? MediaFiles { get; set; }

}

public class CreateMessageDto
{
    /// <summary>
    /// Text content of the message
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Whether the message has been read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Ticket ID that this message belongs to
    /// </summary>
    public int TicketId { get; set; }

    /// <summary>
    /// Sender ID of the message
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// Single media file reference for this message
    /// </summary>
    public List<IFormFile>? MediaFiles { get; set; }
    }

    public class UpdateMessageDto : BaseDto
    {
        /// <summary>
        /// Text content of the message
        /// </summary>
        public required string Text { get; set; }

        /// <summary>
        /// Whether the message has been read
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Ticket ID that this message belongs to
        /// </summary>
        public int TicketId { get; set; }

        /// <summary>
        /// Sender ID of the message
        /// </summary>
        public int SenderId { get; set; }

    /// <summary>
    /// Single media file reference for this message
    /// </summary>
    public List<MediaFileDto>? MediaFiles { get; set; }
    public List<IFormFile>? NewMediaFiles { get; set; }
}



