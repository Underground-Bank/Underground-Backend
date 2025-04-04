using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UndergroundBank.HistoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustHistoryInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DestinationId",
                table: "OperationsHistory",
                newName: "DestinationLoanId");

            migrationBuilder.AddColumn<string>(
                name: "DestinationAccountNumber",
                table: "OperationsHistory",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinationType",
                table: "OperationsHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationAccountNumber",
                table: "OperationsHistory");

            migrationBuilder.DropColumn(
                name: "DestinationType",
                table: "OperationsHistory");

            migrationBuilder.RenameColumn(
                name: "DestinationLoanId",
                table: "OperationsHistory",
                newName: "DestinationId");
        }
    }
}
