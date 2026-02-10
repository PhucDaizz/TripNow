using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCatalogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "StartingPrice",
                table: "Hotels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_Address_City",
                table: "Hotels",
                column: "Address_City");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_Location",
                table: "Hotels",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_Name",
                table: "Hotels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_OwnerId",
                table: "Hotels",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_Status_Price",
                table: "Hotels",
                columns: new[] { "Status", "StartingPrice" });

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_Status_Rating",
                table: "Hotels",
                columns: new[] { "Status", "Rating" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hotels_Address_City",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_Location",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_Name",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_OwnerId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_Status_Price",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_Status_Rating",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "StartingPrice",
                table: "Hotels");
        }
    }
}
