using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(Guid id);
        Task<List<Course>> GetAllAsync();
        Task AddAsync(Course course);
        void Update(Course course);
        void Delete(Course course);
        Task SaveChangesAsync();
    }
}
