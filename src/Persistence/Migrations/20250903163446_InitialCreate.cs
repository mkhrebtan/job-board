using System;
using System.Collections.Generic;
using Domain.Contexts.ResumePostingContext.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Description_Markdown = table.Column<string>(type: "text", nullable: false),
                    Description_PlainText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber_Number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    PhoneNumber_RegionCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description_Markdown = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: false),
                    Description_PlainText = table.Column<string>(type: "text", nullable: false),
                    Location_Address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Location_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location_Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location_District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Location_Latitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Location_Longitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Location_Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RecruiterInfo_Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RecruiterInfo_FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RecruiterInfo_PhoneNumber_Number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    RecruiterInfo_PhoneNumber_RegionCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Salary_Currency = table.Column<string>(type: "text", nullable: false),
                    Salary_MaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Salary_MinAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Title_Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacancies_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vacancies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecruiterId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyUsers_Users_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeekerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesiredPosition = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    EmploymentTypes = table.Column<IReadOnlyCollection<EmploymentType>>(type: "jsonb", nullable: false),
                    WorkArrangements = table.Column<IReadOnlyCollection<WorkArrangement>>(type: "jsonb", nullable: false),
                    ContactInfo_Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContactInfo_PhoneNumber_Number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    ContactInfo_PhoneNumber_RegionCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Location_Address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Location_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location_Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Location_District = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Location_Latitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Location_Longitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Location_Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PersonalInfo_FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PersonalInfo_LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SalaryExpectation_Currency = table.Column<string>(type: "text", nullable: false),
                    SalaryExpectation_Value = table.Column<decimal>(type: "numeric", nullable: false),
                    SkillsDescription_Markdown = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: false),
                    SkillsDescription_PlainText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resumes_Users_SeekerId",
                        column: x => x.SeekerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstitutionName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Degree = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FieldOfStudy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateRange_EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateRange_StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Education_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageSkill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProficiencyLevel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageSkill_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VacancyApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeekerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoverLetter = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    ApplicationType = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ResumeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyApplications_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyApplications_Users_SeekerId",
                        column: x => x.SeekerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacancyApplications_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkExperience",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateRange_EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateRange_StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description_Markdown = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: false),
                    Description_PlainText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkExperience_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_UserId",
                table: "Account",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NormalizedName",
                table: "Categories",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_CompanyId",
                table: "CompanyUsers",
                column: "CompanyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_RecruiterId",
                table: "CompanyUsers",
                column: "RecruiterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Education_ResumeId",
                table: "Education",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageSkill_ResumeId",
                table: "LanguageSkill",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AccountId",
                table: "RefreshTokens",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_SeekerId",
                table: "Resumes",
                column: "SeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_CategoryId",
                table: "Vacancies",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_CompanyId",
                table: "Vacancies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_ResumeId",
                table: "VacancyApplications",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_SeekerId_VacancyId",
                table: "VacancyApplications",
                columns: new[] { "SeekerId", "VacancyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_VacancyId",
                table: "VacancyApplications",
                column: "VacancyId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperience_ResumeId",
                table: "WorkExperience",
                column: "ResumeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyUsers");

            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropTable(
                name: "LanguageSkill");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "VacancyApplications");

            migrationBuilder.DropTable(
                name: "WorkExperience");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Vacancies");

            migrationBuilder.DropTable(
                name: "Resumes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
