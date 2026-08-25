using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteAndAuditFields : Migration
    {
        private static readonly string[] AllTables =
        [
            "FAQCategories", "Levels", "Lookups", "Permissions", "Roles",
            "Accounts", "MediaFiles", "Packages", "UserProfiles",
            "Courses", "Lessons", "PermissionsRoles", "RefreshTokens",
            "Tickets", "Messages", "MessageMediaFile",
            "PackageHistories", "QuestionFAQs", "UserCourseHistories"
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in AllTables)
            {
                // IsDeleted: new soft-delete flag, default false
                migrationBuilder.AddColumn<bool>(
                    name: "IsDeleted",
                    table: table,
                    type: "bit",
                    nullable: false,
                    defaultValue: false);

                // CreatedBy: nullable FK to Account.Id
                migrationBuilder.AddColumn<int>(
                    name: "CreatedBy",
                    table: table,
                    type: "int",
                    nullable: true);

                // ModifiedBy: nullable FK to Account.Id
                migrationBuilder.AddColumn<int>(
                    name: "ModifiedBy",
                    table: table,
                    type: "int",
                    nullable: true);

                // UpdatedAt: was non-nullable, now nullable
                migrationBuilder.AlterColumn<DateTime>(
                    name: "UpdatedAt",
                    table: table,
                    type: "datetime2",
                    nullable: true,
                    oldClrType: typeof(DateTime),
                    oldType: "datetime2",
                    oldNullable: false);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in AllTables)
            {
                migrationBuilder.DropColumn(name: "IsDeleted", table: table);
                migrationBuilder.DropColumn(name: "CreatedBy", table: table);
                migrationBuilder.DropColumn(name: "ModifiedBy", table: table);

                migrationBuilder.AlterColumn<DateTime>(
                    name: "UpdatedAt",
                    table: table,
                    type: "datetime2",
                    nullable: false,
                    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                    oldClrType: typeof(DateTime),
                    oldType: "datetime2",
                    oldNullable: true);
            }
        }
    }
}
