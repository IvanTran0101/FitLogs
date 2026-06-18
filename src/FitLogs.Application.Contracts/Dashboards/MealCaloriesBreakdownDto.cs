using FitLogs.Foods;

namespace FitLogs.Dashboards;

public class MealCaloriesBreakdownDto
{
    public MealType MealType { get; set; }
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
}