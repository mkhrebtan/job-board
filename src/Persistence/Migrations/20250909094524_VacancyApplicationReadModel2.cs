using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VacancyApplicationReadModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VacancyApplications",
                schema: "Read",
                columns: table => new
                {
                    VacancyApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeekerFirstName = table.Column<string>(type: "text", nullable: false),
                    SeekerLastName = table.Column<string>(type: "text", nullable: false),
                    CoverLetter = table.Column<string>(type: "text", nullable: false),
                    ApplicationType = table.Column<string>(type: "text", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResumeTitle = table.Column<string>(type: "text", nullable: true),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyApplications", x => x.VacancyApplicationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_ResumeId1",
                schema: "Read",
                table: "VacancyApplications",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_VacancyId1",
                schema: "Read",
                table: "VacancyApplications",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacancyApplications",
                schema: "Read");
        }
    }
}
