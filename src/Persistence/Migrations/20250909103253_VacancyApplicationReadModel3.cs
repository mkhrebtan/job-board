using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VacancyApplicationReadModel3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VacancyApplications",
                schema: "Read",
                table: "VacancyApplications");

            migrationBuilder.DropColumn(
                name: "VacancyApplicationId",
                schema: "Read",
                table: "VacancyApplications");

            migrationBuilder.RenameIndex(
                name: "IX_VacancyApplications_VacancyId1",
                schema: "Read",
                table: "VacancyApplications",
                newName: "IX_VacancyApplications_VacancyId");

            migrationBuilder.RenameIndex(
                name: "IX_VacancyApplications_ResumeId1",
                schema: "Read",
                table: "VacancyApplications",
                newName: "IX_VacancyApplications_ResumeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_VacancyApplications_VacancyId",
                schema: "Read",
                table: "VacancyApplications",
                newName: "IX_VacancyApplications_VacancyId1");

            migrationBuilder.RenameIndex(
                name: "IX_VacancyApplications_ResumeId",
                schema: "Read",
                table: "VacancyApplications",
                newName: "IX_VacancyApplications_ResumeId1");

            migrationBuilder.AddColumn<Guid>(
                name: "VacancyApplicationId",
                schema: "Read",
                table: "VacancyApplications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_VacancyApplications",
                schema: "Read",
                table: "VacancyApplications",
                column: "VacancyApplicationId");
        }
    }
}
