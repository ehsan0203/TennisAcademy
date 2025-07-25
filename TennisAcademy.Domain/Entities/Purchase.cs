﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Domain.Entities
{
    public class Purchase
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid? CourseId { get; set; }
        public Course Course { get; set; }

        public Guid? PlanId { get; set; }
        public Plan Plan { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    }

}
