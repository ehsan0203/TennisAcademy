using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<List<Ticket>> GetByUserIdAsync(Guid userId);
        Task<List<Ticket>> GetAllAsync();
        Task AddAsync(Ticket ticket);
        void Update(Ticket ticket);
        Task SaveChangesAsync();
    }
}
