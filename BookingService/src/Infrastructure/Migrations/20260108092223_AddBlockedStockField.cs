using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockedStockField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Inventory",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<int>(
                name: "BlockedStock",
                table: "Inventory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedStock",
                table: "Inventory");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Inventory",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
