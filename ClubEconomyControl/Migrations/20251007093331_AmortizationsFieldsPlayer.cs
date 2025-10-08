using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubEconomyControl.Migrations
{
    /// <inheritdoc />
    public partial class AmortizationsFieldsPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AnualAmortization",
                table: "Players",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainningAmortization",
                table: "Players",
                type: "decimal(65,30)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnualAmortization",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "RemainningAmortization",
                table: "Players");
        }
    }
}
