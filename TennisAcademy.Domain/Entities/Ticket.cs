using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Enums;

namespace TennisAcademy.Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string? FileUrl { get; set; }

        public string? VoiceUrl { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid? CoachId { get; set; }
        public User Coach { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.Waiting;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AnsweredAt { get; set; }
        public string? CoachReplyVoiceUrl { get; set; }
        public string? CoachReplyVideoUrl { get; set; }
        public string? CoachReply { get; set; }
    }

}
