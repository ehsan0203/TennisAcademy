using System;

namespace TennisAcademy.Application.DTOs.Ticket
{
    public class TicketStatisticsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ClosedTickets { get; set; }
    }
}
