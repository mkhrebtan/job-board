using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Users_UserId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Education_Resumes_ResumeId",
                table: "Education");

            migrationBuilder.DropForeignKey(
                name: "FK_LanguageSkill_Resumes_ResumeId",
                table: "LanguageSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Account_AccountId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperience_Resumes_ResumeId",
                table: "WorkExperience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkExperience",
                table: "WorkExperience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LanguageSkill",
                table: "LanguageSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Education",
                table: "Education");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Account",
                table: "Account");

            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "JobPosting");

            migrationBuilder.EnsureSchema(
                name: "Recruitment");

            migrationBuilder.EnsureSchema(
                name: "ResumePosting");

            migrationBuilder.EnsureSchema(
                name: "Application");

            migrationBuilder.RenameTable(
                name: "VacancyApplications",
                newName: "VacancyApplications",
                newSchema: "Application");

            migrationBuilder.RenameTable(
                name: "Vacancies",
                newName: "Vacancies",
                newSchema: "JobPosting");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "Resumes",
                newName: "Resumes",
                newSchema: "ResumePosting");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshTokens",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "CompanyUsers",
                newName: "CompanyUsers",
                newSchema: "Recruitment");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Companies",
                newSchema: "Recruitment");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Categories",
                newSchema: "JobPosting");

            migrationBuilder.RenameTable(
                name: "WorkExperience",
                newName: "WorkExperiences",
                newSchema: "ResumePosting");

            migrationBuilder.RenameTable(
                name: "LanguageSkill",
                newName: "LanguageSkills",
                newSchema: "ResumePosting");

            migrationBuilder.RenameTable(
                name: "Education",
                newName: "Educations",
                newSchema: "ResumePosting");

            migrationBuilder.RenameTable(
                name: "Account",
                newName: "Accounts",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_WorkExperience_ResumeId",
                schema: "ResumePosting",
                table: "WorkExperiences",
                newName: "IX_WorkExperiences_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_LanguageSkill_ResumeId",
                schema: "ResumePosting",
                table: "LanguageSkills",
                newName: "IX_LanguageSkills_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Education_ResumeId",
                schema: "ResumePosting",
                table: "Educations",
                newName: "IX_Educations_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Account_UserId",
                schema: "Identity",
                table: "Accounts",
                newName: "IX_Accounts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkExperiences",
                schema: "ResumePosting",
                table: "WorkExperiences",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LanguageSkills",
                schema: "ResumePosting",
                table: "LanguageSkills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Educations",
                schema: "ResumePosting",
                table: "Educations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                schema: "Identity",
                table: "Accounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_UserId",
                schema: "Identity",
                table: "Accounts",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Resumes_ResumeId",
                schema: "ResumePosting",
                table: "Educations",
                column: "ResumeId",
                principalSchema: "ResumePosting",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LanguageSkills_Resumes_ResumeId",
                schema: "ResumePosting",
                table: "LanguageSkills",
                column: "ResumeId",
                principalSchema: "ResumePosting",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Accounts_AccountId",
                schema: "Identity",
                table: "RefreshTokens",
                column: "AccountId",
                principalSchema: "Identity",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperiences_Resumes_ResumeId",
                schema: "ResumePosting",
                table: "WorkExperiences",
                column: "ResumeId",
                principalSchema: "ResumePosting",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_UserId",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Resumes_ResumeId",
                schema: "ResumePosting",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_LanguageSkills_Resumes_ResumeId",
                schema: "ResumePosting",
                table: "LanguageSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Accounts_AccountId",
                schema: "Identity",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkExperiences_Resumes_ResumeId",
                schema: "ResumePosting",
                table: "WorkExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkExperiences",
                schema: "ResumePosting",
                table: "WorkExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LanguageSkills",
                schema: "ResumePosting",
                table: "LanguageSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Educations",
                schema: "ResumePosting",
                table: "Educations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                schema: "Identity",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "VacancyApplications",
                schema: "Application",
                newName: "VacancyApplications");

            migrationBuilder.RenameTable(
                name: "Vacancies",
                schema: "JobPosting",
                newName: "Vacancies");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "Identity",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Resumes",
                schema: "ResumePosting",
                newName: "Resumes");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                schema: "Identity",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "CompanyUsers",
                schema: "Recruitment",
                newName: "CompanyUsers");

            migrationBuilder.RenameTable(
                name: "Companies",
                schema: "Recruitment",
                newName: "Companies");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "JobPosting",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "WorkExperiences",
                schema: "ResumePosting",
                newName: "WorkExperience");

            migrationBuilder.RenameTable(
                name: "LanguageSkills",
                schema: "ResumePosting",
                newName: "LanguageSkill");

            migrationBuilder.RenameTable(
                name: "Educations",
                schema: "ResumePosting",
                newName: "Education");

            migrationBuilder.RenameTable(
                name: "Accounts",
                schema: "Identity",
                newName: "Account");

            migrationBuilder.RenameIndex(
                name: "IX_WorkExperiences_ResumeId",
                table: "WorkExperience",
                newName: "IX_WorkExperience_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_LanguageSkills_ResumeId",
                table: "LanguageSkill",
                newName: "IX_LanguageSkill_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_ResumeId",
                table: "Education",
                newName: "IX_Education_ResumeId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_UserId",
                table: "Account",
                newName: "IX_Account_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkExperience",
                table: "WorkExperience",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LanguageSkill",
                table: "LanguageSkill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Education",
                table: "Education",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Account",
                table: "Account",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Users_UserId",
                table: "Account",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Education_Resumes_ResumeId",
                table: "Education",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LanguageSkill_Resumes_ResumeId",
                table: "LanguageSkill",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Account_AccountId",
                table: "RefreshTokens",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkExperience_Resumes_ResumeId",
                table: "WorkExperience",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
