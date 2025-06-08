using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Services
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid id);
        Task AddCourseAsync(Course course);
    }
}
