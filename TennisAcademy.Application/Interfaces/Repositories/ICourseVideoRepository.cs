using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Repositories
{
    public interface ICourseVideoRepository
    {
        Task<List<CourseVideo>> GetByCourseIdAsync(Guid courseId);
        Task AddAsync(CourseVideo video);
        Task SaveChangesAsync();
    }
}
