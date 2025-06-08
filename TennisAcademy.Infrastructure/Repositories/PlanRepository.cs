using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Domain.Entities;
using TennisAcademy.Infrastructure.Persistence;

namespace TennisAcademy.Infrastructure.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Plan>> GetAllAsync()
        {
            return await _context.Plans.ToListAsync();
        }

        public async Task AddAsync(Plan plan)
        {
            await _context.Plans.AddAsync(plan);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
