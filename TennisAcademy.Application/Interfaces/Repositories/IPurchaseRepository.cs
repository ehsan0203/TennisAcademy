using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Repositories
{
    public interface IPurchaseRepository
    {
        Task AddAsync(Purchase purchase);
        Task<List<Purchase>> GetByUserIdAsync(Guid userId);
        Task<List<Purchase>> GetAllAsync();

        Task SaveChangesAsync();
    }

}
