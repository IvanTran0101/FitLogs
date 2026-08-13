using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitLogs.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseSetSkipState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSkipped",
                table: "AppExerciseSets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SkippedAt",
                table: "AppExerciseSets",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSkipped",
                table: "AppExerciseSets");

            migrationBuilder.DropColumn(
                name: "SkippedAt",
                table: "AppExerciseSets");
        }
    }
}
