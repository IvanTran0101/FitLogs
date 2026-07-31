using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitLogs.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutSessionCurrentExerciseState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentWorkoutSessionExerciseId",
                table: "AppWorkoutSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppWorkoutSessionExercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "AppWorkoutPlans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkoutSessions_CurrentWorkoutSessionExerciseId",
                table: "AppWorkoutSessions",
                column: "CurrentWorkoutSessionExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkoutSessionExercises_WorkoutPlanExerciseId",
                table: "AppWorkoutSessionExercises",
                column: "WorkoutPlanExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkoutPlans_IsArchived",
                table: "AppWorkoutPlans",
                column: "IsArchived");

            migrationBuilder.AddForeignKey(
                name: "FK_AppWorkoutSessions_AppWorkoutPlans_WorkoutPlanId",
                table: "AppWorkoutSessions",
                column: "WorkoutPlanId",
                principalTable: "AppWorkoutPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppWorkoutSessions_AppWorkoutPlans_WorkoutPlanId",
                table: "AppWorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_AppWorkoutSessions_CurrentWorkoutSessionExerciseId",
                table: "AppWorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_AppWorkoutSessionExercises_WorkoutPlanExerciseId",
                table: "AppWorkoutSessionExercises");

            migrationBuilder.DropIndex(
                name: "IX_AppWorkoutPlans_IsArchived",
                table: "AppWorkoutPlans");

            migrationBuilder.DropColumn(
                name: "CurrentWorkoutSessionExerciseId",
                table: "AppWorkoutSessions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppWorkoutSessionExercises");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "AppWorkoutPlans");
        }
    }
}
