using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompanyUserFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyUsers_CompanyId",
                schema: "Recruitment",
                table: "CompanyUsers");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_CompanyId",
                schema: "Recruitment",
                table: "CompanyUsers",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyUsers_CompanyId",
                schema: "Recruitment",
                table: "CompanyUsers");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_CompanyId",
                schema: "Recruitment",
                table: "CompanyUsers",
                column: "CompanyId",
                unique: true);
        }
    }
}
