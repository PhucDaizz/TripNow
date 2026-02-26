using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceRowVersionWithIntVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Inventory");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Inventory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Inventory");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Inventory",
                type: "longblob",
                nullable: false);
        }
    }
}
