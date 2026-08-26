using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessedPaymentOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedPaymentOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedPaymentOrders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedPaymentOrders_OrderId",
                table: "ProcessedPaymentOrders",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedPaymentOrders");
        }
    }
}
