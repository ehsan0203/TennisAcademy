using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MTA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HealthCondition",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HealthDescription",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VideoContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoContents", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2919));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2920));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2921));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2922));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2638));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2642));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2643));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2644));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2645));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2646));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2647));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2648));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2649));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2650));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2650));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2651));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2652));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2653));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2653));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2654));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2655));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2656));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2657));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2657));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2658));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2659));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2660));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2660));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2661));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2662));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2663));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2664));

            migrationBuilder.InsertData(
                table: "Lookups",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 100, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2664), "RespectOthers", null, "کاربران باید با احترام با یکدیگر رفتار کنند" },
                    { 101, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2665), "NoSpam", null, "ارسال پیام‌های تبلیغاتی یا اسپم مجاز نیست" },
                    { 102, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2666), "FairPlay", null, "رعایت اصول بازی جوانمردانه الزامی است" },
                    { 103, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2667), "ProperContent", null, "انتشار محتوای نامناسب یا توهین‌آمیز ممنوع است" },
                    { 104, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2667), "FollowCoach", null, "پیروی از دستورالعمل‌های مربی در طول تمرینات ضروری است" },
                    { 105, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2668), "NoCheating", null, "هرگونه تقلب یا رفتار غیر ورزشی ممنوع است" },
                    { 106, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2669), "EquipmentCare", null, "رعایت اصول نگهداری و مراقبت از تجهیزات الزامی است" },
                    { 107, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2670), "Attendance", null, "حضور به موقع در کلاس‌ها ضروری است" },
                    { 108, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2671), "NoSmoking", null, "سیگار کشیدن در محوطه آکادمی ممنوع است" },
                    { 109, "SiteRule", new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2671), "RespectFacilities", null, "رعایت نظافت و احترام به امکانات آکادمی الزامی است" }
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2948));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2951));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2952));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2953));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2953));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2954));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2891));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2893));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 17, 34, 25, 998, DateTimeKind.Utc).AddTicks(2894));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoContents");

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

            migrationBuilder.DropColumn(
                name: "HealthCondition",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "HealthDescription",
                table: "UserProfiles");

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3155));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3156));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3157));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3158));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2821));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2825));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2826));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2827));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2828));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2828));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2829));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2830));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2831));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2832));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2832));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2833));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2834));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2835));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2909));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2910));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2911));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2912));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2913));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2913));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2914));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2915));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2916));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2917));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2917));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2918));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2919));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2920));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3187));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3189));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3190));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3191));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3192));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3193));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3119));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3120));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(3121));
        }
    }
}
