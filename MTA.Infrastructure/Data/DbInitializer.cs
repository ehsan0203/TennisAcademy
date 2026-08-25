using MTA.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MTA.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        try
        {
            // اعمال مایگریشن‌ها
            await context.Database.MigrateAsync();

            // -------------------------------
            // Seed base lookup data (Roles, Levels, Permissions, Lookups)
            // -------------------------------
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { Title = "Admin" },
                    new Role { Title = "Coach" },
                    new Role { Title = "Student" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.Levels.AnyAsync())
            {
                context.Levels.AddRange(
                    new Level { Title = "Beginner" },
                    new Level { Title = "Intermediate" },
                    new Level { Title = "Advanced" },
                    new Level { Title = "Professional" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.Permissions.AnyAsync())
            {
                context.Permissions.AddRange(
                    new Permission { Title = "ManageUsers", Description = "Can manage user accounts" },
                    new Permission { Title = "ManageCourses", Description = "Can manage courses" },
                    new Permission { Title = "ManageRoles", Description = "Can manage roles" },
                    new Permission { Title = "ViewAnalytics", Description = "Can view analytics" },
                    new Permission { Title = "CreateContent", Description = "Can create course content" },
                    new Permission { Title = "EnrollCourses", Description = "Can enroll in courses" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.Lookups.AnyAsync())
            {
                context.Lookups.AddRange(
                    new Lookup { Category = "AccountStatus", Key = "Active", Value = "فعال" },
                    new Lookup { Category = "AccountStatus", Key = "Inactive", Value = "غیرفعال" },
                    new Lookup { Category = "AccountStatus", Key = "Suspended", Value = "مسدود" },
                    new Lookup { Category = "CourseStatus", Key = "Draft", Value = "پیش‌نویس" },
                    new Lookup { Category = "CourseStatus", Key = "Published", Value = "منتشر شده" },
                    new Lookup { Category = "CourseStatus", Key = "Archived", Value = "بایگانی" },
                    new Lookup { Category = "CourseStatus", Key = "Suspended", Value = "معلق" },
                    new Lookup { Category = "CourseStatus", Key = "Retired", Value = "منقضی شده" },
                    new Lookup { Category = "MediaType", Key = "Video", Value = "ویدئو" },
                    new Lookup { Category = "MediaType", Key = "Document", Value = "سند" },
                    new Lookup { Category = "MediaType", Key = "Image", Value = "تصویر" },
                    new Lookup { Category = "MediaType", Key = "Audio", Value = "صوت" },
                    new Lookup { Category = "DurationUnit", Key = "Day", Value = "روز" },
                    new Lookup { Category = "DurationUnit", Key = "Week", Value = "هفته" },
                    new Lookup { Category = "DurationUnit", Key = "Month", Value = "ماه" },
                    new Lookup { Category = "FAQCategory", Key = "General", Value = "عمومی" },
                    new Lookup { Category = "FAQCategory", Key = "Payment", Value = "پرداخت" },
                    new Lookup { Category = "FAQCategory", Key = "Technical", Value = "فنی" },
                    new Lookup { Category = "FAQCategory", Key = "Course", Value = "دوره‌ها" },
                    new Lookup { Category = "TicketStatus", Key = "Open", Value = "باز" },
                    new Lookup { Category = "TicketStatus", Key = "Pending", Value = "در انتظار" },
                    new Lookup { Category = "TicketStatus", Key = "Closed", Value = "بسته شده" },
                    new Lookup { Category = "PackageStatus", Key = "Active", Value = "فعال" },
                    new Lookup { Category = "PackageStatus", Key = "Expired", Value = "منقضی" },
                    new Lookup { Category = "PackageStatus", Key = "Pending", Value = "در انتظار" },
                    new Lookup { Category = "UserCourseStatus", Key = "Active", Value = "فعال" },
                    new Lookup { Category = "UserCourseStatus", Key = "Completed", Value = "تکمیل شده" },
                    new Lookup { Category = "UserCourseStatus", Key = "Cancelled", Value = "لغو شده" }
                );
                await context.SaveChangesAsync();
            }

            // -------------------------------
            // Seed RolePermissions
            // -------------------------------
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Title == "Admin");
            var coachRole = await context.Roles.FirstOrDefaultAsync(r => r.Title == "Coach");
            var studentRole = await context.Roles.FirstOrDefaultAsync(r => r.Title == "Student");

            if (adminRole != null && coachRole != null && studentRole != null)
            {
                var allPermissions = await context.Permissions.ToListAsync();
                var existingRolePermissions = await context.PermissionsRoles.ToListAsync();

                var rolePermissions = new List<PermissionsRole>();

                // Admin gets all permissions
                foreach (var permission in allPermissions)
                {
                    if (!existingRolePermissions.Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id))
                    {
                        rolePermissions.Add(new PermissionsRole
                        {
                            RoleId = adminRole.Id,
                            PermissionId = permission.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                // Coach gets specific permissions
                var coachPermissions = allPermissions.Where(p =>
                    p.Title == "ManageCourses" ||
                    p.Title == "CreateContent" ||
                    p.Title == "ViewAnalytics").ToList();

                foreach (var permission in coachPermissions)
                {
                    if (!existingRolePermissions.Any(rp => rp.RoleId == coachRole.Id && rp.PermissionId == permission.Id))
                    {
                        rolePermissions.Add(new PermissionsRole
                        {
                            RoleId = coachRole.Id,
                            PermissionId = permission.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                // Student gets basic permission
                var studentPermissions = allPermissions.Where(p => p.Title == "EnrollCourses").ToList();

                foreach (var permission in studentPermissions)
                {
                    if (!existingRolePermissions.Any(rp => rp.RoleId == studentRole.Id && rp.PermissionId == permission.Id))
                    {
                        rolePermissions.Add(new PermissionsRole
                        {
                            RoleId = studentRole.Id,
                            PermissionId = permission.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                if (rolePermissions.Any())
                    await context.PermissionsRoles.AddRangeAsync(rolePermissions);
            }

            // -------------------------------
            // Create default admin user
            // -------------------------------
            var adminEmail = "admin@mta.com";
            if (!await context.Accounts.AnyAsync(a => a.Email == adminEmail))
            {
                var adminRoles = await context.Roles.FirstAsync(r => r.Title == "Admin");
                var beginnerLevel = await context.Levels.FirstAsync(l => l.Title == "Beginner");
                var activeStatus = await context.Lookups.FirstAsync(l => l.Key == "Active" && l.Category == "AccountStatus");

                var adminAccount = new Account
                {
                    Email = adminEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Status = activeStatus,
                    IsActive = true,
                    RoleId = adminRoles.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Accounts.AddAsync(adminAccount);

                var adminProfile = new UserProfile
                {
                    FirstName = "Admin",
                    LastName = "User",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Experience = 0,
                    SkillLevelId = beginnerLevel.Id,
                    Account = adminAccount,
                    CreatedAt = DateTime.UtcNow
                };

                await context.UserProfiles.AddAsync(adminProfile);
            }

            // -------------------------------
            // Seed FAQ categories
            // -------------------------------
            var faqTitles = new[] { "General", "Courses", "Payments", "Technical" };
            var existingFaqs = await context.FAQCategories.Select(f => f.Title).ToListAsync();

            var faqCategories = faqTitles
                .Where(title => !existingFaqs.Contains(title))
                .Select(title => new FAQCategory
                {
                    Title = title,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

            if (faqCategories.Any())
                await context.FAQCategories.AddRangeAsync(faqCategories);

            // -------------------------------
            // ذخیره تغییرات
            // -------------------------------
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error during database seeding: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
