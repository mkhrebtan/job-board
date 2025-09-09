using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OtherResumeAndVacancyModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyVacancies",
                schema: "Read",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SalaryFrom = table.Column<decimal>(type: "numeric", nullable: true),
                    SalaryTo = table.Column<decimal>(type: "numeric", nullable: true),
                    SalaryCurrency = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyVacancies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "RegisteredVacancies",
                schema: "Read",
                columns: table => new
                {
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    UserFullName = table.Column<string>(type: "text", nullable: false),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    UserPhoneNumber = table.Column<string>(type: "text", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredVacancies", x => x.VacancyId);
                });

            migrationBuilder.CreateTable(
                name: "ResumeListing",
                schema: "Read",
                columns: table => new
                {
                    ResumeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    TotalMonthsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    ExpectedSalary = table.Column<decimal>(type: "numeric", nullable: true),
                    ExpectedSalaryCurrency = table.Column<string>(type: "text", nullable: true),
                    EmploymentTypes = table.Column<List<string>>(type: "text[]", nullable: false),
                    WorkArrangements = table.Column<List<string>>(type: "text[]", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeListing", x => x.ResumeId);
                });

            migrationBuilder.CreateTable(
                name: "VacancyListing",
                schema: "Read",
                columns: table => new
                {
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    CompanyLogoUrl = table.Column<string>(type: "text", nullable: true),
                    SalaryFrom = table.Column<decimal>(type: "numeric", nullable: true),
                    SalaryTo = table.Column<decimal>(type: "numeric", nullable: true),
                    SalaryCurrency = table.Column<string>(type: "text", nullable: true),
                    DescriptionPlainText = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyListing", x => x.VacancyId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserResumes_ResumeId",
                schema: "Read",
                table: "UserResumes",
                column: "ResumeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVacancies_CategoryId",
                schema: "Read",
                table: "CompanyVacancies",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVacancies_City",
                schema: "Read",
                table: "CompanyVacancies",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVacancies_Country",
                schema: "Read",
                table: "CompanyVacancies",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVacancies_District",
                schema: "Read",
                table: "CompanyVacancies",
                column: "District");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVacancies_Region",
                schema: "Read",
                table: "CompanyVacancies",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVacancies_Status",
                schema: "Read",
                table: "CompanyVacancies",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeListing_City",
                schema: "Read",
                table: "ResumeListing",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeListing_Country",
                schema: "Read",
                table: "ResumeListing",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeListing_District",
                schema: "Read",
                table: "ResumeListing",
                column: "District");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeListing_Region",
                schema: "Read",
                table: "ResumeListing",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyListing_CategoryId",
                schema: "Read",
                table: "VacancyListing",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyListing_City",
                schema: "Read",
                table: "VacancyListing",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyListing_Country",
                schema: "Read",
                table: "VacancyListing",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyListing_District",
                schema: "Read",
                table: "VacancyListing",
                column: "District");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyListing_Region",
                schema: "Read",
                table: "VacancyListing",
                column: "Region");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyVacancies",
                schema: "Read");

            migrationBuilder.DropTable(
                name: "RegisteredVacancies",
                schema: "Read");

            migrationBuilder.DropTable(
                name: "ResumeListing",
                schema: "Read");

            migrationBuilder.DropTable(
                name: "VacancyListing",
                schema: "Read");

            migrationBuilder.DropIndex(
                name: "IX_UserResumes_ResumeId",
                schema: "Read",
                table: "UserResumes");
        }
    }
}
