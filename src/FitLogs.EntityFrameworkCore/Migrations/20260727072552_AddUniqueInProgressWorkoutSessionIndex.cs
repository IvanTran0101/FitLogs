using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitLogs.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueInProgressWorkoutSessionIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppWorkoutSessions_UserId",
                table: "AppWorkoutSessions");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkoutSessions_UserId_InProgress",
                table: "AppWorkoutSessions",
                column: "UserId",
                unique: true,
                filter: "\"Status\" = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppWorkoutSessions_UserId_InProgress",
                table: "AppWorkoutSessions");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkoutSessions_UserId",
                table: "AppWorkoutSessions",
                column: "UserId");
        }
    }
}
