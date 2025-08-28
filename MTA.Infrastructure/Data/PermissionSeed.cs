using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace MTA.Domain.Entities
{
    public static class PermissionSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Title = "ManageUsers", Description = "Can manage user accounts" },
                new Permission { Id = 2, Title = "ManageCourses", Description = "Can manage courses" },
                new Permission { Id = 3, Title = "ManageRoles", Description = "Can manage roles" },
                new Permission { Id = 4, Title = "ViewAnalytics", Description = "Can view analytics" },
                new Permission { Id = 5, Title = "CreateContent", Description = "Can create course content" },
                new Permission { Id = 6, Title = "EnrollCourses", Description = "Can enroll in courses" }
            );
        }
    }
}

