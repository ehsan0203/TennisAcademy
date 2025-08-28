using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MTA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EnrolledAt",
                table: "UserCourseHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "UserCourseHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.InsertData(
                table: "Lookups",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 26, "UserCourseStatus", new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2918), "Active", null, "فعال" },
                    { 27, "UserCourseStatus", new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2919), "Completed", null, "تکمیل شده" },
                    { 28, "UserCourseStatus", new DateTime(2025, 8, 23, 7, 54, 47, 191, DateTimeKind.Utc).AddTicks(2920), "Cancelled", null, "لغو شده" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseHistories_StatusId",
                table: "UserCourseHistories",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourseHistories_Lookups_StatusId",
                table: "UserCourseHistories",
                column: "StatusId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCourseHistories_Lookups_StatusId",
                table: "UserCourseHistories");

            migrationBuilder.DropIndex(
                name: "IX_UserCourseHistories_StatusId",
                table: "UserCourseHistories");

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

            migrationBuilder.DropColumn(
                name: "EnrolledAt",
                table: "UserCourseHistories");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "UserCourseHistories");

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6756));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6757));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6758));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6759));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6542));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6546));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6547));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6548));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6548));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6549));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6550));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6551));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6551));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6552));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6553));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6554));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6554));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6555));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6556));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6557));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6558));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6558));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6559));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6560));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6561));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6561));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6562));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6563));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6564));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6784));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6786));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6787));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6788));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6816));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6817));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6727));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6729));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 22, 23, 9, 37, 830, DateTimeKind.Utc).AddTicks(6730));
        }
    }
}
