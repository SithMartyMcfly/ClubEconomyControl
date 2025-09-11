using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubEconomyControl.Migrations
{
    /// <inheritdoc />
    public partial class ModelPlayersFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractEndtDate",
                table: "Players");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ContractStartDate",
                table: "Players",
                type: "date",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ContractEndDate",
                table: "Players",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractEndDate",
                table: "Players");

            migrationBuilder.AlterColumn<int>(
                name: "ContractStartDate",
                table: "Players",
                type: "int",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "ContractEndtDate",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
