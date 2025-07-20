using BuildingBlocks.Exceptions;
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
            var plans = await _planRepository.GetAllAsync();
            if (plans == null || !plans.Any())
                throw new NotFoundException("No plans found.");

            return plans;
        }

        public async Task AddPlanAsync(Plan plan)
        {
            await _planRepository.AddAsync(plan);
            await _planRepository.SaveChangesAsync();
        }
    }
}
