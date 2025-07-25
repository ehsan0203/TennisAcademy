using System;

namespace TennisAcademy.Application.DTOs.CourseVideo
{
    public class CourseVideoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsFreePreview { get; set; }
    }
}
