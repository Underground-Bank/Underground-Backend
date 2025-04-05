using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UndergroundBank.BankAccountService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBankAccountEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "BankAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "BankAccounts");
        }
    }
}
