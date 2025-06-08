using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Services
{
    public class PlanService : IPlanService
    {
        private readonly IPlanRepository _planRepository;

        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<List<Plan>> GetAllPlansAsync()
        {
            return await _planRepository.GetAllAsync();
        }

        public async Task AddPlanAsync(Plan plan)
        {
            await _planRepository.AddAsync(plan);
            await _planRepository.SaveChangesAsync();
        }
    }
}
