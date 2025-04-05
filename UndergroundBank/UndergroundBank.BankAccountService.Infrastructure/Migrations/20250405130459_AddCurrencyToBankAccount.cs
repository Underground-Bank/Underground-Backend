using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UndergroundBank.BankAccountService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "BankAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "BankAccounts");
        }
    }
}
