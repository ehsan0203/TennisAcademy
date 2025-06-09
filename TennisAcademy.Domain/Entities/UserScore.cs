using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Domain.Entities
{
    public class UserScore 
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Credit { get; set; }

        // Navigation (در صورت نیاز)
        public User User { get; set; }
    }
}
