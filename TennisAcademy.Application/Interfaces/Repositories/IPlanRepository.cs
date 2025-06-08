using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Repositories
{
    public interface IPlanRepository
    {
        Task<List<Plan>> GetAllAsync();
        Task AddAsync(Plan plan);
        Task SaveChangesAsync();
    }
}
