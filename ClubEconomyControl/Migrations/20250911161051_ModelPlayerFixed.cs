using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubEconomyControl.Migrations
{
    /// <inheritdoc />
    public partial class ModelPlayerFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_BoughtFromClubId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_SoldToClubId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_BoughtFromClubId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_SoldToClubId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "AmortizationLeft",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "AnnualAmortization",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BoughtFromClubId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SoldToClubId",
                table: "Players");

            migrationBuilder.AddColumn<string>(
                name: "BoughtFromClub",
                table: "Players",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SoldToClub",
                table: "Players",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "isSelled",
                table: "Players",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoughtFromClub",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SoldToClub",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "isSelled",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "AmortizationLeft",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnnualAmortization",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BoughtFromClubId",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoldToClubId",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_BoughtFromClubId",
                table: "Players",
                column: "BoughtFromClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_SoldToClubId",
                table: "Players",
                column: "SoldToClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_BoughtFromClubId",
                table: "Players",
                column: "BoughtFromClubId",
                principalTable: "Clubs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_SoldToClubId",
                table: "Players",
                column: "SoldToClubId",
                principalTable: "Clubs",
                principalColumn: "Id");
        }
    }
}
