using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNameToOwerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SocialNotifications",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Notifications",
                newName: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "SocialNotifications",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Notifications",
                newName: "UserId");
        }
    }
}
