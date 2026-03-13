using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdToAuthorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Posts",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Comments",
                newName: "AuthorId");

            migrationBuilder.AddColumn<sbyte>(
                name: "Type",
                table: "Members",
                type: "tinyint",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Members_FullName",
                table: "Members",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Type",
                table: "Members",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Type_FullName",
                table: "Members",
                columns: new[] { "Type", "FullName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_FullName",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_Type",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_Type_FullName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Posts",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Comments",
                newName: "UserId");
        }
    }
}
