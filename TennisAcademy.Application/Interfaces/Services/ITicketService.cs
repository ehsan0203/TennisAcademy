using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.DTOs.Ticket;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetByUserAsync(Guid userId);
        Task<Ticket?> GetByIdAsync(Guid id);
        Task AddTicketAsync(Ticket ticket);
        Task AnswerTicketAsync(AnswerTicketDto dto);
    }
}
