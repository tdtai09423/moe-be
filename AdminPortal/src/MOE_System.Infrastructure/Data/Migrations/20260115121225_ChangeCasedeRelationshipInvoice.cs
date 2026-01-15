using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOE_System.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCasedeRelationshipInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Enrollments_EnrollmentID",
                table: "Invoices");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Enrollments_EnrollmentID",
                table: "Invoices",
                column: "EnrollmentID",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Enrollments_EnrollmentID",
                table: "Invoices");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Enrollments_EnrollmentID",
                table: "Invoices",
                column: "EnrollmentID",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
