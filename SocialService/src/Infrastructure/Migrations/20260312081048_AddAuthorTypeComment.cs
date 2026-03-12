using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorTypeComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "AuthorType",
                table: "Comments",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorType",
                table: "Comments");
        }
    }
}
