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
            migrationBuilder.DropColumn(
                name: "MessageCount",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "TicketCount",
                table: "Packages",
                newName: "CreditCount");

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
            migrationBuilder.RenameColumn(
                name: "CreditCount",
                table: "Packages",
                newName: "TicketCount");

            migrationBuilder.AddColumn<int>(
                name: "MessageCount",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
