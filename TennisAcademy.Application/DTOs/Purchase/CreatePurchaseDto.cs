using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.Purchase
{
    public class CreatePurchaseDto
    {
        public Guid UserId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? PlanId { get; set; }
    }

}
