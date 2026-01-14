using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOE_System.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSchoolingStatusType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SchoolingStatus",
                table: "AccountHolders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SchoolingStatus",
                table: "AccountHolders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
