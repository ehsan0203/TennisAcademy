using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MTA.Domain.Entities
{
    public static class LookupSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lookup>().HasData(
                // =========================
                // Account Status
                // =========================
                new Lookup { Id = 1, Category = "AccountStatus", Key = "Active", Value = "فعال" },
                new Lookup { Id = 2, Category = "AccountStatus", Key = "Inactive", Value = "غیرفعال" },
                new Lookup { Id = 3, Category = "AccountStatus", Key = "Suspended", Value = "مسدود" },

                // =========================
                // Course Status
                // =========================
                new Lookup { Id = 4, Category = "CourseStatus", Key = "Draft", Value = "پیش‌نویس" },
                new Lookup { Id = 5, Category = "CourseStatus", Key = "Published", Value = "منتشر شده" },
                new Lookup { Id = 6, Category = "CourseStatus", Key = "Archived", Value = "بایگانی" },
                new Lookup { Id = 7, Category = "CourseStatus", Key = "Suspended", Value = "معلق" },
                new Lookup { Id = 8, Category = "CourseStatus", Key = "Retired", Value = "منقضی شده" },

                // =========================
                // Media Type
                // =========================
                new Lookup { Id = 9, Category = "MediaType", Key = "Video", Value = "ویدئو" },
                new Lookup { Id = 10, Category = "MediaType", Key = "Document", Value = "سند" },
                new Lookup { Id = 11, Category = "MediaType", Key = "Image", Value = "تصویر" },
                new Lookup { Id = 12, Category = "MediaType", Key = "Audio", Value = "صوت" },

                // =========================
                // Duration Unit
                // =========================
                new Lookup { Id = 13, Category = "DurationUnit", Key = "Day", Value = "روز" },
                new Lookup { Id = 14, Category = "DurationUnit", Key = "Week", Value = "هفته" },
                new Lookup { Id = 15, Category = "DurationUnit", Key = "Month", Value = "ماه" },

                // =========================
                // FAQ Categories
                // =========================
                new Lookup { Id = 16, Category = "FAQCategory", Key = "General", Value = "عمومی" },
                new Lookup { Id = 17, Category = "FAQCategory", Key = "Payment", Value = "پرداخت" },
                new Lookup { Id = 18, Category = "FAQCategory", Key = "Technical", Value = "فنی" },
                new Lookup { Id = 19, Category = "FAQCategory", Key = "Course", Value = "دوره‌ها" },

                // =========================
                // Ticket Status
                // =========================
                new Lookup { Id = 20, Category = "TicketStatus", Key = "Open", Value = "باز" },
                new Lookup { Id = 21, Category = "TicketStatus", Key = "Pending", Value = "در انتظار" },
                new Lookup { Id = 22, Category = "TicketStatus", Key = "Closed", Value = "بسته شده" },

                // =========================
                // Package Status
                // =========================
                new Lookup { Id = 23, Category = "PackageStatus", Key = "Active", Value = "فعال" },
                new Lookup { Id = 24, Category = "PackageStatus", Key = "Expired", Value = "منقضی" },
                new Lookup { Id = 25, Category = "PackageStatus", Key = "Pending", Value = "در انتظار" },

                // =========================
                // UserCourseHistory Status
                // =========================
                new Lookup { Id = 26, Category = "UserCourseStatus", Key = "Active", Value = "فعال" },
                new Lookup { Id = 27, Category = "UserCourseStatus", Key = "Completed", Value = "تکمیل شده" },
                new Lookup { Id = 28, Category = "UserCourseStatus", Key = "Cancelled", Value = "لغو شده" }

            );
        }
    }
}
