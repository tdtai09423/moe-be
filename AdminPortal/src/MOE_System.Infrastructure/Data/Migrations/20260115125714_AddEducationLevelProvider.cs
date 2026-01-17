using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOE_System.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEducationLevelProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "Providers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "Providers");
        }
    }
}
