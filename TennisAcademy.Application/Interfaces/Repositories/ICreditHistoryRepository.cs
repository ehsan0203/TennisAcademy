using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Repositories
{
    public interface ICreditHistoryRepository
    {
        Task AddAsync(CreditHistory history);
        Task<List<CreditHistory>> GetAllAsync();
        Task<List<CreditHistory>> GetByUserIdAsync(Guid userId);
    }

}
