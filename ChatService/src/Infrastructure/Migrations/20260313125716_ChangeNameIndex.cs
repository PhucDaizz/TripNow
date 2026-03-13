using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Members_Type_FullName",
                table: "ChatProfiles",
                newName: "IX_ChatProfiles_Type_FullName");

            migrationBuilder.RenameIndex(
                name: "IX_Members_Type",
                table: "ChatProfiles",
                newName: "IX_ChatProfiles_Type");

            migrationBuilder.RenameIndex(
                name: "IX_Members_FullName",
                table: "ChatProfiles",
                newName: "IX_ChatProfiles_FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_ChatProfiles_Type_FullName",
                table: "ChatProfiles",
                newName: "IX_Members_Type_FullName");

            migrationBuilder.RenameIndex(
                name: "IX_ChatProfiles_Type",
                table: "ChatProfiles",
                newName: "IX_Members_Type");

            migrationBuilder.RenameIndex(
                name: "IX_ChatProfiles_FullName",
                table: "ChatProfiles",
                newName: "IX_Members_FullName");
        }
    }
}
