using System;

namespace FitLogs.Foods.FoodLogs;

public class DailyFoodNutritionSummaryDto
{
    public DateTime Date { get; set; }

    public decimal TotalCalories { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarb { get; set; }
    public decimal TotalFat { get; set; }
}