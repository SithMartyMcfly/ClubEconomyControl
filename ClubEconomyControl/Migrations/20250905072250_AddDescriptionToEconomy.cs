using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubEconomyControl.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToEconomy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OrdinaryIncomes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OrdinaryExpenses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExtraordinaryIncomes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExtraordinaryExpenses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "OrdinaryIncomes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "OrdinaryExpenses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExtraordinaryIncomes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExtraordinaryExpenses");
        }
    }
}
