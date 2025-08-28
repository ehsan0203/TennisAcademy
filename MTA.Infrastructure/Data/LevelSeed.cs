using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MTA.Domain.Entities
{
    public static class LevelSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Level>().HasData(
                new Level { Id = 1, Title = "Beginner" },
                new Level { Id = 2, Title = "Intermediate" },
                new Level { Id = 3, Title = "Advanced" },
                new Level { Id = 4, Title = "Professional" }
            );
        }
    }
}

