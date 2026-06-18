using System.Collections.Generic;
using FitLogs.Dashboards;

public class DailyNutritionSummaryDto
{
    public bool HasNutritionData { get; set; }

    public decimal? DailyCaloriesTarget { get; set; }

    public bool HasCaloriesTarget { get; set; }

    public decimal TotalCalories { get; set; }

    public decimal? RemainingCalories { get; set; }

    public decimal TotalProtein { get; set; }

    public decimal TotalCarbs { get; set; }

    public decimal TotalFat { get; set; }

    public List<MealCaloriesBreakdownDto> CaloriesByMealType { get; set; } = new();
}