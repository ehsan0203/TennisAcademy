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
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ApplicationDbContext _context;

        public PurchaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Purchase purchase)
        {
            await _context.Purchases.AddAsync(purchase);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Purchase>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(p => p.Course)
                .Include(p => p.Plan)
                .ToListAsync();
        }
        public async Task<List<Purchase>> GetAllAsync()
        {
            return await _context.Purchases
                .Include(p => p.Course)
                .Include(p => p.Plan)
                .ToListAsync();
        }

    }

}
