using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Services
{
    public interface IPlanService
    {
        Task<List<Plan>> GetAllPlansAsync();
        Task AddPlanAsync(Plan plan);
    }
}
