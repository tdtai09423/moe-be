using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOE_System.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeePerCycleToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FeePerCycle",
                table: "Courses",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeePerCycle",
                table: "Courses");
        }
    }
}
