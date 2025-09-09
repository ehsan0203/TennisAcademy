using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                // Account Status (1-3)
                // =========================
                new Lookup { Id = 1, Category = "AccountStatus", Key = "Active", Value = "فعال" },
                new Lookup { Id = 2, Category = "AccountStatus", Key = "Inactive", Value = "غیرفعال" },
                new Lookup { Id = 3, Category = "AccountStatus", Key = "Suspended", Value = "مسدود" },

                // =========================
                // Course Status (4-8)
                // =========================
                new Lookup { Id = 4, Category = "CourseStatus", Key = "Draft", Value = "پیش‌نویس" },
                new Lookup { Id = 5, Category = "CourseStatus", Key = "Published", Value = "منتشر شده" },
                new Lookup { Id = 6, Category = "CourseStatus", Key = "Archived", Value = "بایگانی" },
                new Lookup { Id = 7, Category = "CourseStatus", Key = "Suspended", Value = "معلق" },
                new Lookup { Id = 8, Category = "CourseStatus", Key = "Retired", Value = "منقضی شده" },

                // =========================
                // Media Type (9-12)
                // =========================
                new Lookup { Id = 9, Category = "MediaType", Key = "Video", Value = "ویدئو" },
                new Lookup { Id = 10, Category = "MediaType", Key = "Document", Value = "سند" },
                new Lookup { Id = 11, Category = "MediaType", Key = "Image", Value = "تصویر" },
                new Lookup { Id = 12, Category = "MediaType", Key = "Audio", Value = "صوت" },

                // =========================
                // Duration Unit (13-15)
                // =========================
                new Lookup { Id = 13, Category = "DurationUnit", Key = "Day", Value = "روز" },
                new Lookup { Id = 14, Category = "DurationUnit", Key = "Week", Value = "هفته" },
                new Lookup { Id = 15, Category = "DurationUnit", Key = "Month", Value = "ماه" },

                // =========================
                // FAQ Categories (16-19)
                // =========================
                new Lookup { Id = 16, Category = "FAQCategory", Key = "General", Value = "عمومی" },
                new Lookup { Id = 17, Category = "FAQCategory", Key = "Payment", Value = "پرداخت" },
                new Lookup { Id = 18, Category = "FAQCategory", Key = "Technical", Value = "فنی" },
                new Lookup { Id = 19, Category = "FAQCategory", Key = "Course", Value = "دوره‌ها" },

                // =========================
                // Ticket Status (20-22)
                // =========================
                new Lookup { Id = 20, Category = "TicketStatus", Key = "Open", Value = "باز" },
                new Lookup { Id = 21, Category = "TicketStatus", Key = "Pending", Value = "در انتظار" },
                new Lookup { Id = 22, Category = "TicketStatus", Key = "Closed", Value = "بسته شده" },

                // =========================
                // Package Status (23-25)
                // =========================
                new Lookup { Id = 23, Category = "PackageStatus", Key = "Active", Value = "فعال" },
                new Lookup { Id = 24, Category = "PackageStatus", Key = "Expired", Value = "منقضی" },
                new Lookup { Id = 25, Category = "PackageStatus", Key = "Pending", Value = "در انتظار" },

                // =========================
                // UserCourseHistory Status (26-28)
                // =========================
                new Lookup { Id = 26, Category = "UserCourseStatus", Key = "Active", Value = "فعال" },
                new Lookup { Id = 27, Category = "UserCourseStatus", Key = "Completed", Value = "تکمیل شده" },
                new Lookup { Id = 28, Category = "UserCourseStatus", Key = "Cancelled", Value = "لغو شده" },

                // =========================
                // Site Rules (100-109)
                // =========================
                new Lookup { Id = 100, Category = "SiteRule", Key = "RespectOthers", Value = "کاربران باید با احترام با یکدیگر رفتار کنند" },
                new Lookup { Id = 101, Category = "SiteRule", Key = "NoSpam", Value = "ارسال پیام‌های تبلیغاتی یا اسپم مجاز نیست" },
                new Lookup { Id = 102, Category = "SiteRule", Key = "FairPlay", Value = "رعایت اصول بازی جوانمردانه الزامی است" },
                new Lookup { Id = 103, Category = "SiteRule", Key = "ProperContent", Value = "انتشار محتوای نامناسب یا توهین‌آمیز ممنوع است" },
                new Lookup { Id = 104, Category = "SiteRule", Key = "FollowCoach", Value = "پیروی از دستورالعمل‌های مربی در طول تمرینات ضروری است" },
                new Lookup { Id = 105, Category = "SiteRule", Key = "NoCheating", Value = "هرگونه تقلب یا رفتار غیر ورزشی ممنوع است" },
                new Lookup { Id = 106, Category = "SiteRule", Key = "EquipmentCare", Value = "رعایت اصول نگهداری و مراقبت از تجهیزات الزامی است" },
                new Lookup { Id = 107, Category = "SiteRule", Key = "Attendance", Value = "حضور به موقع در کلاس‌ها ضروری است" },
                new Lookup { Id = 108, Category = "SiteRule", Key = "NoSmoking", Value = "سیگار کشیدن در محوطه آکادمی ممنوع است" },
                new Lookup { Id = 109, Category = "SiteRule", Key = "RespectFacilities", Value = "رعایت نظافت و احترام به امکانات آکادمی الزامی است" },

                // =========================
                // Media Placement (200-204)
                // =========================
                new Lookup { Id = 200, Category = "MediaPlacement", Key = "WelcomeVideo", Value = "ویدئوی خوشامدگویی" },
                new Lookup { Id = 201, Category = "MediaPlacement", Key = "PromoVideo", Value = "ویدئوی تبلیغاتی" },
                new Lookup { Id = 202, Category = "MediaPlacement", Key = "SliderVideo", Value = "ویدئو اسلایدر" },
                new Lookup { Id = 203, Category = "MediaPlacement", Key = "TrainingPage", Value = "ویدئو صفحه آموزش" },
                new Lookup { Id = 204, Category = "MediaPlacement", Key = "CoursePage", Value = "ویدئو صفحه دوره" }

            );
        }
    }
}
