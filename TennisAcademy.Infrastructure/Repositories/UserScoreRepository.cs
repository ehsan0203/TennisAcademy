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
    public class UserScoreRepository : IUserScoreRepository
    {
        private readonly ApplicationDbContext _context;

        public UserScoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserScore> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserScores
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task AddOrUpdateAsync(UserScore userScore)
        {
            var existing = await _context.UserScores
                .FirstOrDefaultAsync(x => x.UserId == userScore.UserId);

            if (existing == null)
            {
                _context.UserScores.Add(userScore);
            }
            else
            {
                existing.Credit = userScore.Credit;
                _context.UserScores.Update(existing);
            }

            await _context.SaveChangesAsync();
        }
    }

}
