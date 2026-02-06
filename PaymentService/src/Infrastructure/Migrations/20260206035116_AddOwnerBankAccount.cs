using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payouts_SettlementPeriods_SettlementId",
                table: "Payouts");

            migrationBuilder.AddColumn<Guid>(
                name: "SettlementPeriodId",
                table: "WalletLedgers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<decimal>(
                name: "TransactionFee",
                table: "WalletLedgers",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TransactionGrossAmount",
                table: "WalletLedgers",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerWalletId",
                table: "Payouts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "OwnerBankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OwnerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BankName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankAccountNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankAccountHolder = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedBy = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerBankAccounts", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerBankAccounts_OwnerId",
                table: "OwnerBankAccounts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerBankAccounts_OwnerId_IsDefault",
                table: "OwnerBankAccounts",
                columns: new[] { "OwnerId", "IsDefault" });

            migrationBuilder.AddForeignKey(
                name: "FK_Payouts_SettlementPeriods_SettlementId",
                table: "Payouts",
                column: "SettlementId",
                principalTable: "SettlementPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payouts_SettlementPeriods_SettlementId",
                table: "Payouts");

            migrationBuilder.DropTable(
                name: "OwnerBankAccounts");

            migrationBuilder.DropColumn(
                name: "SettlementPeriodId",
                table: "WalletLedgers");

            migrationBuilder.DropColumn(
                name: "TransactionFee",
                table: "WalletLedgers");

            migrationBuilder.DropColumn(
                name: "TransactionGrossAmount",
                table: "WalletLedgers");

            migrationBuilder.DropColumn(
                name: "OwnerWalletId",
                table: "Payouts");

            migrationBuilder.AddForeignKey(
                name: "FK_Payouts_SettlementPeriods_SettlementId",
                table: "Payouts",
                column: "SettlementId",
                principalTable: "SettlementPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
