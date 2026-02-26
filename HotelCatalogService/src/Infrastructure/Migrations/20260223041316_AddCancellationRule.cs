using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCatalogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancellationRule_CancellationPolicies_CancellationPolicyId",
                table: "CancellationRule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CancellationRule",
                table: "CancellationRule");

            migrationBuilder.RenameTable(
                name: "CancellationRule",
                newName: "CancellationRules");

            migrationBuilder.RenameIndex(
                name: "IX_CancellationRule_CancellationPolicyId",
                table: "CancellationRules",
                newName: "IX_CancellationRules_CancellationPolicyId");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "CancellationRules",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "RefundPercentage",
                table: "CancellationRules",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "CancellationRules",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CancellationRules",
                table: "CancellationRules",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CancellationRules_CancellationPolicies_CancellationPolicyId",
                table: "CancellationRules",
                column: "CancellationPolicyId",
                principalTable: "CancellationPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CancellationRules_CancellationPolicies_CancellationPolicyId",
                table: "CancellationRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CancellationRules",
                table: "CancellationRules");

            migrationBuilder.RenameTable(
                name: "CancellationRules",
                newName: "CancellationRule");

            migrationBuilder.RenameIndex(
                name: "IX_CancellationRules_CancellationPolicyId",
                table: "CancellationRule",
                newName: "IX_CancellationRule_CancellationPolicyId");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "CancellationRule",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "RefundPercentage",
                table: "CancellationRule",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "CancellationRule",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CancellationRule",
                table: "CancellationRule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CancellationRule_CancellationPolicies_CancellationPolicyId",
                table: "CancellationRule",
                column: "CancellationPolicyId",
                principalTable: "CancellationPolicies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
