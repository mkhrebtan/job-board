using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VacancyListingSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VacancyListing_CompanyName",
                schema: "Read",
                table: "VacancyListing",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyListing_DescriptionPlainText",
                schema: "Read",
                table: "VacancyListing",
                column: "DescriptionPlainText");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VacancyListing_CompanyName",
                schema: "Read",
                table: "VacancyListing");

            migrationBuilder.DropIndex(
                name: "IX_VacancyListing_DescriptionPlainText",
                schema: "Read",
                table: "VacancyListing");
        }
    }
}
