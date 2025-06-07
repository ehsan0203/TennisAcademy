using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Domain.Entities
{
    public class CourseVideo
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string VideoUrl { get; set; }

        public bool IsFreePreview { get; set; }

        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }

}
