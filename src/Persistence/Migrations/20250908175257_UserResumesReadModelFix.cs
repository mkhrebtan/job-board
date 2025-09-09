using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserResumesReadModelFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserResumes",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.DropColumn(
                name: "Resumes",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "Read",
                table: "UserResumes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "Read",
                table: "UserResumes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                schema: "Read",
                table: "UserResumes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ResumeId",
                schema: "Read",
                table: "UserResumes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "Read",
                table: "UserResumes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserResumes",
                schema: "Read",
                table: "UserResumes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserResumes_UserId",
                schema: "Read",
                table: "UserResumes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserResumes",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.DropIndex(
                name: "IX_UserResumes_UserId",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.DropColumn(
                name: "ResumeId",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "Read",
                table: "UserResumes");

            migrationBuilder.AddColumn<List<UserResume>>(
                name: "Resumes",
                schema: "Read",
                table: "UserResumes",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserResumes",
                schema: "Read",
                table: "UserResumes",
                column: "UserId");
        }
    }
}
