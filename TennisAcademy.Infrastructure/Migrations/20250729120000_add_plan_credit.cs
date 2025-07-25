using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TennisAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_plan_credit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Credit",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "Plans");
        }
    }
}
