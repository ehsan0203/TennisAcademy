using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoIntroUrl { get; set; }

        public string CoverImageUrl { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<CourseVideo> Videos { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
    }

}
