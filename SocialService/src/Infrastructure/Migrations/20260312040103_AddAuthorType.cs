using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Hotel_Feed",
                table: "Posts");

            migrationBuilder.AddColumn<sbyte>(
                name: "AuthorType",
                table: "Posts",
                type: "tinyint",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Hotel_Feed",
                table: "Posts",
                columns: new[] { "HotelId", "AuthorType", "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Hotel_Feed",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "AuthorType",
                table: "Posts");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Hotel_Feed",
                table: "Posts",
                columns: new[] { "HotelId", "Status", "CreatedAt" });
        }
    }
}
