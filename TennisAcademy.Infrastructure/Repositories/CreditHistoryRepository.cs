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
    public class CreditHistoryRepository : ICreditHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CreditHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CreditHistory history)
        {
            _context.CreditHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CreditHistory>> GetAllAsync()
        {
            return await _context.CreditHistories
                .Include(h => h.User)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<CreditHistory>> GetByUserIdAsync(Guid userId)
        {
            return await _context.CreditHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }
    }

}
