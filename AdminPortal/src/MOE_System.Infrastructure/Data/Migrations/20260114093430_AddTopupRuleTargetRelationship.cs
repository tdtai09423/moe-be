using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOE_System.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTopupRuleTargetRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RuleTargetType",
                table: "TopupRules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetEducationAccountId",
                table: "TopupRules",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearningType",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TopupRules_TargetEducationAccountId",
                table: "TopupRules",
                column: "TargetEducationAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_TopupRules_EducationAccounts_TargetEducationAccountId",
                table: "TopupRules",
                column: "TargetEducationAccountId",
                principalTable: "EducationAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TopupRules_EducationAccounts_TargetEducationAccountId",
                table: "TopupRules");

            migrationBuilder.DropIndex(
                name: "IX_TopupRules_TargetEducationAccountId",
                table: "TopupRules");

            migrationBuilder.DropColumn(
                name: "RuleTargetType",
                table: "TopupRules");

            migrationBuilder.DropColumn(
                name: "TargetEducationAccountId",
                table: "TopupRules");

            migrationBuilder.DropColumn(
                name: "LearningType",
                table: "Courses");
        }
    }
}
