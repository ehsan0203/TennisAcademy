using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Repositories
{
    public interface ICoachMediaRepository
    {
        Task<List<CoachMedia>> GetByCoachIdAsync(Guid coachId);

        Task AddAsync(CoachMedia media);
        Task<CoachMedia?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();

    }
}
