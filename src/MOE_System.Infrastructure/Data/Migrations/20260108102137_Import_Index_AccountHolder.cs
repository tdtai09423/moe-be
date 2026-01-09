using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOE_System.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Import_Index_AccountHolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AccountHolders_Email",
                table: "AccountHolders",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolders_NRIC",
                table: "AccountHolders",
                column: "NRIC",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountHolders_Email",
                table: "AccountHolders");

            migrationBuilder.DropIndex(
                name: "IX_AccountHolders_NRIC",
                table: "AccountHolders");
        }
    }
}
