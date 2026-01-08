using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCatalogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldMaintainDateForRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "MaintenanceEnd",
                table: "Rooms",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "MaintenanceStart",
                table: "Rooms",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceEnd",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MaintenanceStart",
                table: "Rooms");
        }
    }
}
