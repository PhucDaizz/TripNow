using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCatalogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldFollower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Follower",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Follower",
                table: "Hotels");
        }
    }
}
