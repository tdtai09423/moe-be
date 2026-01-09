using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOE_System.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_AccountHolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "AccountHolders",
                newName: "RegisteredAddress");

            migrationBuilder.AddColumn<string>(
                name: "MailingAddress",
                table: "AccountHolders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MailingAddress",
                table: "AccountHolders");

            migrationBuilder.RenameColumn(
                name: "RegisteredAddress",
                table: "AccountHolders",
                newName: "Address");
        }
    }
}
