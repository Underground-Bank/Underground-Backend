using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UndergroundBank.LoanService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanTopUpJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TopUpJobs",
                columns: table => new
                {
                    BankAccountNumber = table.Column<string>(type: "text", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopUpJobs", x => new { x.BankAccountNumber, x.LoanId, x.UserId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopUpJobs");
        }
    }
}
