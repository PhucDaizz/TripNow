using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCatalogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMinBookingAmountField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinBookingAmount",
                table: "Promotions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinBookingAmount",
                table: "Promotions");
        }
    }
}
