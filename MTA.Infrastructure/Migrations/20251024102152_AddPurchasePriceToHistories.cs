using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchasePriceToHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionFAQs_FAQCategories_CategoryId",
                table: "QuestionFAQs");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "UserCourseHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "QuestionFAQs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "PackageHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionFAQs_FAQCategories_CategoryId",
                table: "QuestionFAQs",
                column: "CategoryId",
                principalTable: "FAQCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionFAQs_FAQCategories_CategoryId",
                table: "QuestionFAQs");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "UserCourseHistories");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "PackageHistories");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "QuestionFAQs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionFAQs_FAQCategories_CategoryId",
                table: "QuestionFAQs",
                column: "CategoryId",
                principalTable: "FAQCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
