using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCatalogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoomTypeImages_PublicId",
                table: "RoomTypeImages",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_FloorId_RoomName",
                table: "Rooms",
                columns: new[] { "FloorId", "RoomName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId_Status",
                table: "Rooms",
                columns: new[] { "RoomTypeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomPrices_RoomTypeId_Date",
                table: "RoomPrices",
                columns: new[] { "RoomTypeId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_HotelId_IsActive",
                table: "Promotions",
                columns: new[] { "HotelId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_HotelImages_PublicId",
                table: "HotelImages",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Floors_BlockId_FloorNumber",
                table: "Floors",
                columns: new[] { "BlockId", "FloorNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancellationRules_PolicyId_Hours",
                table: "CancellationRules",
                columns: new[] { "CancellationPolicyId", "HoursBeforeCheckIn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancellationPolicies_HotelId",
                table: "CancellationPolicies",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_HotelId_Name",
                table: "Blocks",
                columns: new[] { "HotelId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Amenities_Name",
                table: "Amenities",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomTypeImages_PublicId",
                table: "RoomTypeImages");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_FloorId_RoomName",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomTypeId_Status",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_RoomPrices_RoomTypeId_Date",
                table: "RoomPrices");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_HotelId_IsActive",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_HotelImages_PublicId",
                table: "HotelImages");

            migrationBuilder.DropIndex(
                name: "IX_Floors_BlockId_FloorNumber",
                table: "Floors");

            migrationBuilder.DropIndex(
                name: "IX_CancellationRules_PolicyId_Hours",
                table: "CancellationRules");

            migrationBuilder.DropIndex(
                name: "IX_CancellationPolicies_HotelId",
                table: "CancellationPolicies");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_HotelId_Name",
                table: "Blocks");

            migrationBuilder.DropIndex(
                name: "IX_Amenities_Name",
                table: "Amenities");
        }
    }
}
