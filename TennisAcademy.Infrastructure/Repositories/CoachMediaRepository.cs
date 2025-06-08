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
    public class CoachMediaRepository : ICoachMediaRepository
    {
        private readonly ApplicationDbContext _context;

        public CoachMediaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CoachMedia?> GetByIdAsync(Guid id)
        {
            return await _context.CoachMedia.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<CoachMedia>> GetByCoachIdAsync(Guid coachId)
        {
            return await _context.CoachMedia
                .Where(m => m.CoachId == coachId)
                .ToListAsync();
        }

        public async Task AddAsync(CoachMedia media)
        {
            await _context.CoachMedia.AddAsync(media);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
