using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MTA.Domain.Entities
{
    public static class RoleSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Title = "Admin" },
                new Role { Id = 2, Title = "Coach" },
                new Role { Id = 3, Title = "Student" }
            );
        }
    }
}

