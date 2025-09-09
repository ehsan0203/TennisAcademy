using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MTA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Levels",
                columns: new[] { "Id", "CreatedAt", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5840), "Beginner", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5842), "Intermediate", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5843), "Advanced", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5843), "Professional", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Lookups",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, "AccountStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5536), "Active", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "فعال" },
                    { 2, "AccountStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5540), "Inactive", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "غیرفعال" },
                    { 3, "AccountStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5541), "Suspended", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "مسدود" },
                    { 4, "CourseStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5542), "Draft", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "پیش‌نویس" },
                    { 5, "CourseStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5543), "Published", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "منتشر شده" },
                    { 6, "CourseStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5544), "Archived", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بایگانی" },
                    { 7, "CourseStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5545), "Suspended", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "معلق" },
                    { 8, "CourseStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5545), "Retired", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "منقضی شده" },
                    { 9, "MediaType", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5546), "Video", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ویدئو" },
                    { 10, "MediaType", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5547), "Document", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "سند" },
                    { 11, "MediaType", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5548), "Image", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "تصویر" },
                    { 12, "MediaType", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5548), "Audio", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "صوت" },
                    { 13, "DurationUnit", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5549), "Day", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "روز" },
                    { 14, "DurationUnit", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5550), "Week", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "هفته" },
                    { 15, "DurationUnit", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5551), "Month", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ماه" },
                    { 16, "FAQCategory", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5551), "General", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "عمومی" },
                    { 17, "FAQCategory", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5552), "Payment", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "پرداخت" },
                    { 18, "FAQCategory", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5553), "Technical", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "فنی" },
                    { 19, "FAQCategory", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5554), "Course", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "دوره‌ها" },
                    { 20, "TicketStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5555), "Open", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "باز" },
                    { 21, "TicketStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5555), "Pending", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "در انتظار" },
                    { 22, "TicketStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5556), "Closed", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "بسته شده" },
                    { 23, "PackageStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5557), "Active", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "فعال" },
                    { 24, "PackageStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5558), "Expired", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "منقضی" },
                    { 25, "PackageStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5558), "Pending", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "در انتظار" },
                    { 26, "UserCourseStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5559), "Active", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "فعال" },
                    { 27, "UserCourseStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5560), "Completed", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "تکمیل شده" },
                    { 28, "UserCourseStatus", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5561), "Cancelled", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "لغو شده" },
                    { 100, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5562), "RespectOthers", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "کاربران باید با احترام با یکدیگر رفتار کنند" },
                    { 101, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5562), "NoSpam", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ارسال پیام‌های تبلیغاتی یا اسپم مجاز نیست" },
                    { 102, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5563), "FairPlay", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رعایت اصول بازی جوانمردانه الزامی است" },
                    { 103, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5564), "ProperContent", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "انتشار محتوای نامناسب یا توهین‌آمیز ممنوع است" },
                    { 104, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5565), "FollowCoach", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "پیروی از دستورالعمل‌های مربی در طول تمرینات ضروری است" },
                    { 105, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5565), "NoCheating", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "هرگونه تقلب یا رفتار غیر ورزشی ممنوع است" },
                    { 106, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5566), "EquipmentCare", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رعایت اصول نگهداری و مراقبت از تجهیزات الزامی است" },
                    { 107, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5567), "Attendance", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "حضور به موقع در کلاس‌ها ضروری است" },
                    { 108, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5568), "NoSmoking", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "سیگار کشیدن در محوطه آکادمی ممنوع است" },
                    { 109, "SiteRule", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5568), "RespectFacilities", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رعایت نظافت و احترام به امکانات آکادمی الزامی است" },
                    { 200, "MediaPlacement", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5569), "WelcomeVideo", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ویدئوی خوشامدگویی" },
                    { 201, "MediaPlacement", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5570), "PromoVideo", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ویدئوی تبلیغاتی" },
                    { 202, "MediaPlacement", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5571), "SliderVideo", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ویدئو اسلایدر" },
                    { 203, "MediaPlacement", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5571), "TrainingPage", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ویدئو صفحه آموزش" },
                    { 204, "MediaPlacement", new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5572), "CoursePage", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ویدئو صفحه دوره" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5868), "Can manage user accounts", "ManageUsers", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5870), "Can manage courses", "ManageCourses", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5871), "Can manage roles", "ManageRoles", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5872), "Can view analytics", "ViewAnalytics", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5873), "Can create course content", "CreateContent", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5874), "Can enroll in courses", "EnrollCourses", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5801), "Admin", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5803), "Coach", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2025, 9, 9, 11, 12, 58, 571, DateTimeKind.Utc).AddTicks(5804), "Student", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
