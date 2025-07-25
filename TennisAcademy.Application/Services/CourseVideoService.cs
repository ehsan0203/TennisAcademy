using BuildingBlocks.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Services
{
    public class CourseVideoService : ICourseVideoService
    {
        private readonly ICourseVideoRepository _videoRepo;
        private readonly IPurchaseRepository _purchaseRepo;

        public CourseVideoService(ICourseVideoRepository videoRepo, IPurchaseRepository purchaseRepo)
        {
            _videoRepo = videoRepo;
            _purchaseRepo = purchaseRepo;
        }

        public async Task<List<CourseVideo>> GetVideosForUserAsync(Guid courseId, Guid? userId)
        {
            var videos = await _videoRepo.GetByCourseIdAsync(courseId);
            if (videos == null || !videos.Any())
                throw new NotFoundException("No videos found for this course.");

            if (userId.HasValue)
            {
                var purchases = await _purchaseRepo.GetByUserIdAsync(userId.Value);
                var hasCourse = purchases.Any(p => p.CourseId == courseId);
                if (hasCourse)
                    return videos;
            }

            return videos.Where(v => v.IsFreePreview).ToList();
        }

        public async Task AddAsync(CourseVideo video)
        {
            await _videoRepo.AddAsync(video);
            await _videoRepo.SaveChangesAsync();
        }
    }
}
