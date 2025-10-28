using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackagesForCredits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Lookups_DurationUnitId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_DurationUnitId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DurationUnitId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "MessageCount",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "TicketCount",
                table: "Packages",
                newName: "CreditCount");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Packages",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.RenameColumn(
                name: "RemainingTickets",
                table: "PackageHistories",
                newName: "RemainingCredits");

            migrationBuilder.DropColumn(
                name: "RemainingMessages",
                table: "PackageHistories");

            migrationBuilder.AddColumn<int>(
                name: "TotalCredits",
                table: "PackageHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE PackageHistories SET TotalCredits = RemainingCredits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "CreditCount",
                table: "Packages",
                newName: "TicketCount");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationUnitId",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessageCount",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_DurationUnitId",
                table: "Packages",
                column: "DurationUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Lookups_DurationUnitId",
                table: "Packages",
                column: "DurationUnitId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "TotalCredits",
                table: "PackageHistories");

            migrationBuilder.AddColumn<int>(
                name: "RemainingMessages",
                table: "PackageHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.RenameColumn(
                name: "RemainingCredits",
                table: "PackageHistories",
                newName: "RemainingTickets");
        }
    }
}
