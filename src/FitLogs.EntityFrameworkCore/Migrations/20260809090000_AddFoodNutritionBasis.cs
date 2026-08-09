using FitLogs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitLogs.Migrations;

/// <summary>Adds explicit nutrition denominators and immutable calculation metadata.</summary>
[DbContext(typeof(FitLogsDbContext))]
[Migration("20260809090000_AddFoodNutritionBasis")]
public partial class AddFoodNutritionBasis : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "NutritionBasisAmount",
            table: "AppFoodProducts",
            type: "numeric(18,2)",
            nullable: false,
            defaultValue: 100m);

        migrationBuilder.AddColumn<int>(
            name: "NutritionBasisUnit",
            table: "AppFoodProducts",
            type: "integer",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.AddColumn<decimal?>(
            name: "ServingAmount",
            table: "AppFoodProducts",
            type: "numeric(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<int?>(
            name: "ServingUnit",
            table: "AppFoodProducts",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<decimal?>(
            name: "PieceWeightGrams",
            table: "AppFoodProducts",
            type: "numeric(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "NutritionBasisAmount",
            table: "AppFoodLogs",
            type: "numeric(18,2)",
            nullable: false,
            defaultValue: 100m);

        migrationBuilder.AddColumn<int>(
            name: "NutritionBasisUnit",
            table: "AppFoodLogs",
            type: "integer",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.AddColumn<decimal>(
            name: "NutritionConversionFactor",
            table: "AppFoodLogs",
            type: "numeric(18,6)",
            nullable: false,
            defaultValue: 1m);

        migrationBuilder.AddColumn<int>(
            name: "NutritionCalculationSource",
            table: "AppFoodLogs",
            type: "integer",
            nullable: false,
            defaultValue: 3);

        // Legacy logs were calculated as quantity / 100, so preserve their historical factor explicitly.
        migrationBuilder.Sql("UPDATE \"AppFoodLogs\" SET \"NutritionConversionFactor\" = \"Quantity\" / 100.0;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "NutritionBasisAmount", table: "AppFoodProducts");
        migrationBuilder.DropColumn(name: "NutritionBasisUnit", table: "AppFoodProducts");
        migrationBuilder.DropColumn(name: "ServingAmount", table: "AppFoodProducts");
        migrationBuilder.DropColumn(name: "ServingUnit", table: "AppFoodProducts");
        migrationBuilder.DropColumn(name: "PieceWeightGrams", table: "AppFoodProducts");
        migrationBuilder.DropColumn(name: "NutritionBasisAmount", table: "AppFoodLogs");
        migrationBuilder.DropColumn(name: "NutritionBasisUnit", table: "AppFoodLogs");
        migrationBuilder.DropColumn(name: "NutritionConversionFactor", table: "AppFoodLogs");
        migrationBuilder.DropColumn(name: "NutritionCalculationSource", table: "AppFoodLogs");
    }
}
