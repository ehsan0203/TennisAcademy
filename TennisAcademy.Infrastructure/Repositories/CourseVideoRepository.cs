using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Domain.Entities;
using TennisAcademy.Infrastructure.Persistence;

namespace TennisAcademy.Infrastructure.Repositories
{
    public class CourseVideoRepository : ICourseVideoRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseVideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CourseVideo>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.CourseVideos
                .Where(v => v.CourseId == courseId)
                .ToListAsync();
        }

        public async Task AddAsync(CourseVideo video)
        {
            await _context.CourseVideos.AddAsync(video);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
