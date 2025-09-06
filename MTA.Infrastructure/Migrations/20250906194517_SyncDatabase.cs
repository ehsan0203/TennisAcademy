using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4330));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4331));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4332));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4333));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4084));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4088));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4088));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4089));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4090));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4091));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4092));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4093));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4093));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4094));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4095));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4096));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4096));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4097));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4098));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4099));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4099));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4100));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4101));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4102));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4103));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4103));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4104));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4105));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4106));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4107));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4107));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4108));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4109));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4110));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4111));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4111));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4112));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4113));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4114));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4114));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4115));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4116));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4367));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4404));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4405));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4406));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4407));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4408));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4289));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4291));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 19, 45, 17, 331, DateTimeKind.Utc).AddTicks(4292));

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AccountId",
                table: "RefreshTokens",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7361));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7362));

            migrationBuilder.UpdateData(
                table: "Levels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7363));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7043));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7046));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7047));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7048));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7049));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7050));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7051));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7051));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7052));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7053));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7054));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7054));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7055));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7056));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7057));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7058));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7059));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7059));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7060));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7061));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7062));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7062));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7063));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7064));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7146));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7148));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7149));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7149));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7150));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7151));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7152));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7152));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7153));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7154));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7155));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7156));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7156));

            migrationBuilder.UpdateData(
                table: "Lookups",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7157));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7387));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7390));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7391));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7392));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7393));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7393));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7329));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7331));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 6, 18, 14, 43, 81, DateTimeKind.Utc).AddTicks(7332));
        }
    }
}
