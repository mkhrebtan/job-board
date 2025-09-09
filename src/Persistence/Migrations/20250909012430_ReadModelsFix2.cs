using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReadModelsFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyVacancies",
                schema: "Read",
                table: "CompanyVacancies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyVacancies",
                schema: "Read",
                table: "CompanyVacancies",
                column: "VacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVacancies_CompanyId",
                schema: "Read",
                table: "CompanyVacancies",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyVacancies",
                schema: "Read",
                table: "CompanyVacancies");

            migrationBuilder.DropIndex(
                name: "IX_CompanyVacancies_CompanyId",
                schema: "Read",
                table: "CompanyVacancies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyVacancies",
                schema: "Read",
                table: "CompanyVacancies",
                column: "CompanyId");
        }
    }
}
