using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubEconomyControl.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceEconomyCodeAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceCode",
                table: "OrdinaryIncomes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceCode",
                table: "OrdinaryExpenses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceCode",
                table: "ExtraordinaryIncomes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceCode",
                table: "ExtraordinaryExpenses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceCode",
                table: "OrdinaryIncomes");

            migrationBuilder.DropColumn(
                name: "ReferenceCode",
                table: "OrdinaryExpenses");

            migrationBuilder.DropColumn(
                name: "ReferenceCode",
                table: "ExtraordinaryIncomes");

            migrationBuilder.DropColumn(
                name: "ReferenceCode",
                table: "ExtraordinaryExpenses");
        }
    }
}
