using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Domain.Entities
{
    public class CreditHistory
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Amount { get; set; } // منفی یا مثبت
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }

}
