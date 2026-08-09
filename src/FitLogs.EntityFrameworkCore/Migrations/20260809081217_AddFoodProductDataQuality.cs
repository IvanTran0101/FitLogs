using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitLogs.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodProductDataQuality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CaloriesPer100g",
                table: "AppFoodProducts",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "DataQuality",
                table: "AppFoodProducts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE "AppFoodProducts"
                SET "DataQuality" = CASE
                    WHEN "CaloriesPer100g" IS NOT NULL
                     AND "ProteinPer100g" IS NOT NULL
                     AND "CarbPer100g" IS NOT NULL
                     AND "FatPer100g" IS NOT NULL THEN 2
                    WHEN "CaloriesPer100g" IS NOT NULL
                      OR "ProteinPer100g" IS NOT NULL
                      OR "CarbPer100g" IS NOT NULL
                      OR "FatPer100g" IS NOT NULL THEN 1
                    ELSE 0
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataQuality",
                table: "AppFoodProducts");

            migrationBuilder.AlterColumn<decimal>(
                name: "CaloriesPer100g",
                table: "AppFoodProducts",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);
        }
    }
}
