using System;
using TennisAcademy.Domain.Enums;

namespace TennisAcademy.Application.DTOs.Ticket
{
    public class AdminTicketDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public TicketStatus Status { get; set; }
        public string? CoachName { get; set; }
        public string? FileUrl { get; set; }
        public string? VoiceUrl { get; set; }
        public string? CoachReplyVoiceUrl { get; set; }
        public string? CoachReplyVideoUrl { get; set; }
        public string? CoachReply { get; set; }
    }
}
