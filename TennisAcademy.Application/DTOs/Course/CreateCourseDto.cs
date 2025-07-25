using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.Course
{
    public class CreateCourseDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string CoverImageUrl { get; set; }

        public string VideoIntroUrl { get; set; }

        public int TotalDurationMinutes { get; set; }

        public decimal Price { get; set; }
    }
}
