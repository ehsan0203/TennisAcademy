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
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            var result = await _courseRepository.GetAllAsync();
            if (result == null || !result.Any())
              throw new NotFoundException("Course list is empty.");
            return result;
        }

        public async Task<Course?> GetCourseByIdAsync(Guid id)
        {
            var result = await _courseRepository.GetByIdAsync(id);
           if (result == null)
                throw new NotFoundException("Course not found.");
            return result;
        }

        public async Task AddCourseAsync(Course course)
        {
            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();
        }
    }
}
