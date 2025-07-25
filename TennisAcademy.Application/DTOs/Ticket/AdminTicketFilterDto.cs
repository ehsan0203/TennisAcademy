using System;
using TennisAcademy.Domain.Enums;

namespace TennisAcademy.Application.DTOs.Ticket
{
    public class AdminTicketFilterDto
    {
        public string? Query { get; set; }
        public TicketStatus? Status { get; set; }
        public string? Subject { get; set; }
        public Guid? CoachId { get; set; }
        public DateTime? FromCreated { get; set; }
        public DateTime? ToCreated { get; set; }
        public DateTime? FromAnswered { get; set; }
        public DateTime? ToAnswered { get; set; }
    }
}
