using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOE_System.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResidentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    NRIC = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PrincipalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Race = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryRace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dialect = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    ResidentialStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.NRIC);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Residents");
        }
    }
}
