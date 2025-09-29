using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents a message in the system
/// </summary>
public class Message : BaseEntity
{
    public required string Text { get; set; }
    public bool IsRead { get; set; } = false;
    
    public int TicketId { get; set; }
    [ForeignKey("TicketId")]
    public virtual Ticket Ticket { get; set; } = null!;
    
    public int SenderId { get; set; }
    [ForeignKey(nameof(SenderId))]
    public virtual Account Sender { get; set; } = null!;

    public virtual ICollection<MessageMediaFile> MediaFiles { get; set; } = new List<MessageMediaFile>();

    //public int? MediaFileId { get; set; }
    //[ForeignKey(nameof(MediaFileId))]
    //public virtual MediaFile? MediaFile { get; set; }

}

public class MessageMediaFile : BaseEntity
{
    public int MessageId { get; set; }
    public virtual Message Message { get; set; } = null!;

    public int MediaFileId { get; set; }
    public virtual MediaFile MediaFile { get; set; } = null!;
}


