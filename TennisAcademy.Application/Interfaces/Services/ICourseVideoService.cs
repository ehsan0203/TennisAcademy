using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Services
{
    public interface ICourseVideoService
    {
        Task<List<CourseVideo>> GetVideosForUserAsync(Guid courseId, Guid? userId);
        Task AddAsync(CourseVideo video);
    }
}
