using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.Ticket
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string? FileUrl { get; set; }
        public string? VoiceUrl { get; set; }
        public string? CoachReply { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
